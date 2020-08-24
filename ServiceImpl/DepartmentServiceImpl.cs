﻿using BackEndAD.DataContext;
using BackEndAD.Models;
using BackEndAD.Repo;
using BackEndAD.ServiceInterface;
using BackEndAD.TempService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndAD.ServiceImpl
{
    public class DepartmentServiceImpl : IDepartmentService
    {
        public IUnitOfWork<ProjectContext> unitOfWork;
        

        public DepartmentServiceImpl(IUnitOfWork<ProjectContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region department
        public async Task<IList<Department>> findAllDepartmentsAsync()
        {
            IList<Department> deptlist = await unitOfWork.GetRepository<Department>().GetAllAsync();
            return deptlist;
        }

        public async Task<Department> findDepartmentByIdAsync(int deptId)
        {
            //for repo
            //Department dept = await deptrepo.findDepartmentByIdAsync(id);
            Department dept = await unitOfWork.GetRepository<Department>().FindAsync(deptId);
            return dept;
        }
        public IList<Department> findAllDepartmentsAsyncEager()
        {
            IList<Department> deptlist = 
                unitOfWork.GetRepository<Department>()
                .GetAllIncludeIQueryable(null, null,"Collection").ToList();
            /*
            IList<Department> deptlist = await
                unitOfWork.GetRepository<Department>().GetAllAsync(null,null,
                    s => s.Include(de => de.Collection).ThenInclude(coll => coll.Id)
                    );*/
            return deptlist;
        }
        #endregion

        #region requsition
        public async Task<IList<Requisition>> findAllRequsitionsAsync()
        {
            IList<Requisition> reqlist = await unitOfWork.GetRepository<Requisition>().GetAllAsync();
            return reqlist;
        }
        #endregion

        #region requisition details
        public async Task<IList<RequisitionDetail>> findAllRequsitionDetailAsync()
        {
            IList<RequisitionDetail> detailsLists = await unitOfWork.GetRepository<RequisitionDetail>().GetAllAsync();
            return detailsLists;
        }

        public async Task<IList<RequisitionDetailsList>> findAllRequisitionDetailsItemListById(Requisition req)
        {
            IList<RequisitionDetailsList> reqDList = new List<RequisitionDetailsList>();
            // IList<RequisitionDetail> reqDetail = await findAllRequsitionDetailAsync();
            IList<Stationery> stationery = await findAllStationeryAsync();

            //retrieve list of req detail with equal id
            IList<RequisitionDetail> rList = unitOfWork
                .GetRepository<RequisitionDetail>()
                .GetAllIncludeIQueryable(filter: x => x.RequisitionId == req.Id).ToList();

            foreach (RequisitionDetail reqDetailRecord in rList)
            {
                foreach (Stationery sItem in stationery)
                {
                    if (reqDetailRecord.StationeryId == sItem.Id)
                    {
                        RequisitionDetailsList requisition = new RequisitionDetailsList()
                        {
                            requisitionDetailsId = reqDetailRecord.Id,
                            requisitionId = reqDetailRecord.RequisitionId,
                            description = sItem.desc,
                            quantity = reqDetailRecord.reqQty,
                            unit = sItem.unit,
                            status = reqDetailRecord.status
                        };
                        reqDList.Add(requisition);
                    }
                }
            }
            return reqDList;
        }
        #endregion

        #region requisition apply
        public async Task<IList<RequisitionDetailsApply>> findRequisitiondetailsApply(RequisitionDetailsApply req)
        {
            IList<RequisitionDetailsApply> reqLists = await unitOfWork.GetRepository<RequisitionDetailsApply>().GetAllAsync();
            return reqLists;
        }

        public async Task<IList<Requisition>> applyRequisition(List<RequisitionDetailsApply> reqList)
        {
                Requisition requisition = new Requisition()
                {
                    EmployeeId = 1,
                    dateOfRequest = DateTime.Now,
                    dateOfAuthorizing = DateTime.Now,
                    AuthorizerId = 1,
                    status = "Applied",
                };
                unitOfWork.GetRepository<Requisition>().Insert(requisition);
                unitOfWork.SaveChanges();

            foreach (RequisitionDetailsApply reqDetails in reqList)
            {

                Stationery stationeries = unitOfWork
                    .GetRepository<Stationery>()
                    .GetAllIncludeIQueryable(filter: x => x.desc == reqDetails.desc).FirstOrDefault();

                RequisitionDetail reqDetail1 = new RequisitionDetail()
                {
                    RequisitionId = requisition.Id,
                    StationeryId = stationeries.Id,
                    reqQty = reqDetails.reqQty,
                    status = "Applied",
                };
                unitOfWork.GetRepository<RequisitionDetail>().Insert(reqDetail1);
                unitOfWork.SaveChanges();
            }
            
            return await findAllRequsitionsAsync();
        }

        public async Task<IList<RequisitionDetailsApply>> viewRequisitionApplyRow()
        {
            List<RequisitionDetailsApply> reqApply = new List<RequisitionDetailsApply>();

            IList<RequisitionDetail> reqDetail = unitOfWork
              .GetRepository<RequisitionDetail>()
              .GetAllIncludeIQueryable(filter: x => x.status == "Applied" && x.Requisition.status == "Applied").ToList();

            foreach (var detail in reqDetail)
            {
                if (detail != null)
                {
                    Stationery stationery = unitOfWork
                          .GetRepository<Stationery>()
                          .GetAllIncludeIQueryable(filter: x => x.Id == detail.StationeryId).FirstOrDefault();
                    RequisitionDetailsApply apply = new RequisitionDetailsApply()
                    {
                        requisitionDetailId = detail.Id,
                        category = stationery.category,
                        desc = stationery.desc,
                        reqQty = detail.reqQty,
                        unit = stationery.unit,
                    };

                    reqApply.Add(apply);

                }

            }
            return reqApply;
        }

        public async Task<IList<RequisitionDetailsApply>> viewRequisitionApply(Requisition requisition)
        {
            List<RequisitionDetailsApply> reqApply = new List<RequisitionDetailsApply>();

            IList<RequisitionDetail> reqDetail = unitOfWork
              .GetRepository<RequisitionDetail>()
              .GetAllIncludeIQueryable(filter: x => x.RequisitionId == requisition.Id && x.status == "Applied" && requisition.status == "Applied").ToList();

            foreach (var detail in reqDetail)
            {
                if (detail != null)
                {
                    Stationery stationery = unitOfWork
                          .GetRepository<Stationery>()
                          .GetAllIncludeIQueryable(filter: x => x.Id == detail.StationeryId).FirstOrDefault();
                    RequisitionDetailsApply apply = new RequisitionDetailsApply()
                    {
                        requisitionDetailId = detail.Id,
                        category = stationery.category,
                        desc = stationery.desc,
                        reqQty = detail.reqQty,
                        unit = stationery.unit,
                    };

                    reqApply.Add(apply);

                }
            }

            return reqApply;
        }



        public void deleteRequisitionDetail(int id)
        {
            unitOfWork.GetRepository<RequisitionDetail>().Delete(id);
            unitOfWork.SaveChanges();
        }
        #endregion

        #region stationery
        public async Task<IList<Stationery>> findAllStationeryAsync()
        {
            IList<Stationery> stationeryList = await unitOfWork.GetRepository<Stationery>().GetAllAsync();
            return stationeryList;
        }
        #endregion

        #region Employee
        public async Task<Employee> findEmployeeByIdAsync(int empid)
        {
            Employee emp = await unitOfWork.GetRepository<Employee>().FindAsync(empid);
            return emp;
        }
        public async Task<IList<Employee>> findAllEmployeesAsync()
        {
            IList<Employee> emplist = await unitOfWork.GetRepository<Employee>().GetAllAsync();
            //IIncludableQueryable<TEntity, object>> include = null,
            return emplist;
        }

        public async Task<IList<CollectionInfo>> findAllCollectionPointAsync()
        {
            IList<CollectionInfo> collectionpts = await unitOfWork.GetRepository<CollectionInfo>().GetAllAsync();

            return collectionpts;
        }
        #endregion


    }
}
