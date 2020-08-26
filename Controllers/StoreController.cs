﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndAD.TempService;
using BackEndAD.Models;
using BackEndAD.ServiceInterface;
using System;
using System.Linq;
using System.Data.SqlClient;

//REMINDER: All existing comments generated by BiancaZYCao
//This is an simple example about how to code Web API controller return data result for ReactJS
//
namespace BackEndAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private IStoreClerkService _clkService;
        private IEmailService _emailService;
        private IStoreManagerService _mgrService;
        private IStoreSupervisorService _supervisorService;
        public StoreController(IEmailService emailService, IStoreClerkService clkService, IStoreManagerService mgrService, IStoreSupervisorService supervisorService)
        {
            _clkService = clkService;
            _mgrService = mgrService;
            _supervisorService = supervisorService;
            _emailService = emailService;
        }

        #region Stationery List (Inventory)
        [HttpGet("Stationeries")]
        public async Task<ActionResult<List<Stationery>>> GetAllStationeries()
        {
            var result = await _clkService.findAllStationeriesAsync();
            // if find data then return result else will return a String says Department not found
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                return NotFound("Stationeries not found");
        }

        //Post Request for stationery by id
        [HttpPost("Stationery/post")]
        public Task<ActionResult<Stationery>> PostStationery([FromBody] Stationery stationery)
        {
            Console.WriteLine("stationaryPost");
            _clkService.saveStationery(stationery);
            return null;
        }
        [HttpDelete("Stationery/delete/{id}")]
        public Task<ActionResult<Stationery>> DeleteStationery(int id)
        {
            _clkService.deleteStationery(id);
            return null;
        }

        [HttpGet("Stationeries/{id}")]
        public async Task<ActionResult<Stationery>> GetStationeryByIdAsync(int id)
        {
            var result = await _clkService.findStationeryByIdAsync(id);
            // if find data then return result else will return a String says Department not found
            if (result != null)
                return Ok(result);
            else
                return NotFound("Stationery not found");
        }
        #endregion

        #region Get Suppliers + CRUD
        [HttpGet("Suppliers")]
        public async Task<ActionResult<List<Supplier>>> GetAllSuppliers()
        {
            var result = await _clkService.findAllSuppliersAsync();
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Suppliers not found");
        }

        [HttpPost("saveSupplier")]
        public async Task<ActionResult<Supplier>> saveSupplier([FromBody] Supplier s)
        {
            Supplier sup = new Supplier()
            {
                supplierCode = s.supplierCode,
                name = s.name,
                contactPerson = s.contactPerson,
                email = s.email,
                phoneNum = s.phoneNum,
                gstRegisNo = s.gstRegisNo,
                fax = s.fax,
                address = s.address,
                priority = s.priority,
            };
            _clkService.saveSupplier(sup);

            return CreatedAtAction(nameof(GetAllSuppliers), new { }, sup);
        }

        [HttpPost("deleteSupplier")]
        public async Task<ActionResult<Supplier>> DeleteSupplier([FromBody] Supplier s)
        {
            _clkService.deleteSupplier(s.Id);
            return CreatedAtAction(nameof(GetAllSuppliers), new { }, s);
        }

        [HttpPost("updateSupplier")]
        public async Task<ActionResult<Supplier>> UpdateSupplier([FromBody] Supplier sup)
        {

            _clkService.updateSupplier(sup);
            return CreatedAtAction(nameof(GetAllSuppliers), new { }, sup);

        }
        #endregion

        #region inventory management tasks: adjustment + voucher
        //Clerk
        [HttpGet("getAllRequesterRow")]
        public async Task<ActionResult<List<StockAdjustSumById>>> GetAllRequesterRow()
        {

            var result = await _clkService.GetAllRequesterRow();
            if (result != null)
            {
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                Console.WriteLine(result[0].representativeName);
                return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }


        [HttpPost("getDisburseItemDetail")]
        public async Task<ActionResult<List<DisburseItemDetails>>> GetDisburseItemDetail([FromBody] RequesterRow row)
        {
            var result = await _clkService.getDisburseItemDetail(row);
            if (result != null)
            {
                //String str = await _emailService.SendMail("theingi@gmail.com", "Email Testing", "This is to test email service...");
                return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }

        //Supervisor
        [HttpGet("supervisorAdjustment")]
        public async Task<ActionResult<List<StockAdjustSumById>>> GetAllSupervisorAdustmentInfo()
        {

            var result = await _supervisorService.StockAdjustDetailInfo();
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }

        //StoreManager stockadjustment voucher
        [HttpGet("adjustmentList")]
        public async Task<ActionResult<List<StockAdjustSumById>>> GetAllAdustmentInfo()
        {

            var result = await _mgrService.StockAdjustDetailInfo();
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Empty");
        }


        [HttpPost("getAllAdjustDetailLine")]
        public async Task<ActionResult<List<AdjustmentVocherInfo>>> getAllAdjustDetailLine([FromBody] StockAdjustSumById item)
        {
            var result = await _mgrService.getAllAdjustDetailLineByAdjustId(item);
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }


        [HttpPost("getAllSupervisorAdjustDetailLine")]
        public async Task<ActionResult<List<AdjustmentVocherInfo>>> getAllSupervisorAdjustDetailLine([FromBody] StockAdjustSumById item)
        {
            var result = await _supervisorService.getAllAdjustDetailLineByAdjustId(item);
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }


        [HttpPost("supervisorRejectRequest/{comment}")]
        public async Task<ActionResult<List<StockAdjustSumById>>> RejectRequest([FromBody] StockAdjustSumById voc,String comment)
        {

            var result = await _supervisorService.rejectRequest(voc,comment);
            Employee emp = await _mgrService.findEmployeeByIdAsync(voc.empId);
            //String emailBody = "Stock Adjustment Form #" + voc.stockAdustmentId + " is rejected. Could you please check again.";
            //String str = await _emailService.SendMail(emp.email, "Rejected:Stock Adjustment Form #" + voc.stockAdustmentId, emailBody);
           
            if (result != null)
            {
                return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Empty");
        }

        [HttpPost("managerRejectRequest/{comment}")]
        public async Task<ActionResult<List<StockAdjustSumById>>> ManagerRejectRequest([FromBody] StockAdjustSumById voc,String comment)
        {

            var result = await _mgrService.rejectRequest(voc,comment);
            Employee emp = await _mgrService.findEmployeeByIdAsync(voc.empId);
            //String emailBody = "Stock Adjustment Form #" + voc.stockAdustmentId + " is rejected. Could you please check again.";
            //String str = await _emailService.SendMail(emp.email, "Rejected:Stock Adjustment Form #" + voc.stockAdustmentId, emailBody);
            //String str = await _emailService.SendMail(emp.email, "Rejected:Stock Adjustment Form #" , "Reject");

            if (result != null)
            {
               return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Empty");
        }

        [HttpPost("issueVoucher")]
        public async Task<ActionResult<List<AdjustmentVocherInfo>>> CreateVoucher([FromBody] StockAdjustSumById voc)
        {
            var result = await _mgrService.issueVoucher(voc);
           /* Employee emp = await _mgrService.findEmployeeByIdAsync(voc.empId);
            String emailBody = "Stock Adjustment Form #" + voc.stockAdustmentId + " has been Approved.";
            //String str = await _emailService.SendMail(emp.email, "Approved:Stock Adjustment Form #" + voc.stockAdustmentId, emailBody);
           */ 
            if (result != null)
            {
               return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");

        }


        [HttpPost("supervisorissueVoucher")]
        public async Task<ActionResult<List<AdjustmentVocherInfo>>> SupervisorissueVoucher([FromBody] StockAdjustSumById voc)
        {
            var result = await _supervisorService.issueVoucher(voc);

            Employee emp = await _supervisorService.findEmployeeByIdAsync(voc.empId);
            String emailBody = "Stock Adjustment Form #" + voc.stockAdustmentId + " has been Approved.";
            //await _emailService.SendMail(emp.email, "Approved:Stock Adjustment Form #" + voc.stockAdustmentId, emailBody);
            //Console.WriteLine(str);
            if (result != null)
            {
                 return Ok(result);
            }
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");

        }

        [HttpPost("getVoucher")]
        public async Task<ActionResult<AdjustmentVocherInfo>> getVoucher([FromBody] AdjustmentVocherInfo voc)
        {
            var result = await _mgrService.getEachVoucherDetail(voc);
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Error");
        }

        [HttpGet("retrieval/{id}")]
        public async Task<ActionResult<IList<RequisitionDetail>>> GetAllPendingRequisitions(int id)
        {
            var clerk = await _clkService.findEmployeeByIdAsync(id);
            var collectionpoints = await _clkService.findAllCollectionPointAsync();
            var currCollectionpoint = collectionpoints.Where(x => x.clerkId == clerk.Id);
            var currCollectionpoint2 = currCollectionpoint.Select(x => x.Id);
            var departments = await _clkService.findAllDepartmentAsync();
            var currDepartments = departments.Where(x => currCollectionpoint2.Contains(x.CollectionId));
            var currDepartments2 = currDepartments.Select(x => x.Id);
            var employees = await _clkService.findEmployeesAsync();
            var currEmployees = employees.Where(x => currDepartments2.Contains(x.departmentId));
            var currEmployees2 = currEmployees.Select(x => x.Id);

            var requisitions = await _clkService.findAllRequsitionAsync();
            var allRD = await _clkService.findAllRequsitionDetailsAsync();

            var nonDeliveredRD = allRD.Where(x => x.status != "Delivered");
            var nonDeclinedRD = nonDeliveredRD.Where(x => x.status != "Declined");
            var nonApprovedRD = nonDeclinedRD.Where(x => x.status != "Applied");
            var result = nonApprovedRD.Where(x => x.reqQty != x.rcvQty);

            var requisition = result.Select(x => x.RequisitionId);
            HashSet<int> uniqueRequisitionID = new HashSet<int>();
            foreach(int i in requisition)
            {
                uniqueRequisitionID.Add(i);
            }
            List<Requisition> currentRequisitions = new List<Requisition>();
            
            foreach (Requisition r in requisitions)
            {
                foreach(int i in uniqueRequisitionID)
                {
                    if (r.Id == i && currEmployees2.Contains(r.EmployeeId))
                    {
                        currentRequisitions.Add(r);
                    }
                }
            }
            var currentRequisitions2 = currentRequisitions.Select(x => x.Id);
            var result2 = result.Where(x => currentRequisitions2.Contains(x.RequisitionId));
            if (result2 != null)
            {
                //convert to json file
                Console.WriteLine("android called for pending retrievals");
                return Ok(result2);
            }
            else
                //in case there is nothing to process
                return NotFound("No pending requistions");
        }



        [HttpPost("getRetrieval")]
        public async Task<ActionResult<Requisition>> processRetrieval(
              [FromBody] fakeRequisition requistitions)
        {
            var result = await _clkService.findAllRequsitionDetailsAsync();
            Console.WriteLine("post");
            Console.WriteLine(requistitions);
            return Ok(requistitions);
        }

        [HttpPost("processRetrieval")]
        public async Task<ActionResult<DisbursementList>> processRetrieval(
              [FromBody] List<fakeRequisitionDetails> fakeRequisitions)
        {
            Console.WriteLine(fakeRequisitions.First().requisitionId);

            #region data fetching for processing
            Console.WriteLine("post");
            var allRD = await _clkService.findAllRequsitionDetailsAsync();
            var nonDeliveredRD = allRD.Where(x => x.status != "Delivered");
            var nonAppliedRD = nonDeliveredRD.Where(x => x.status != "Applied");
            var requisitiondetails = nonAppliedRD.Where(x => x.status != "Declined");
            var requisitions = await _clkService.findAllRequsitionAsync();
            var stationeries = await _clkService.findAllStationeriesAsync();
            var departments = await _clkService.findAllDepartmentAsync();
            var collectionpoints = await _clkService.findAllCollectionPointAsync();
            Console.WriteLine("fetching done and starting processing");
            #endregion

            #region create new Stock Adjustment
            StockAdjustment newSA = new StockAdjustment();
            newSA.date = DateTime.Now;
            newSA.EmployeeId = fakeRequisitions.First().requisitionId;
            newSA.type = "stock retrieval";
            _clkService.saveStockAdjustment(newSA);
            Console.WriteLine("created stock adjustment");
            #endregion

            #region creating necessary Disbursement List
            HashSet<Department> deptlist = new HashSet<Department>();
            foreach (fakeRequisitionDetails i in fakeRequisitions)
            {
                foreach (RequisitionDetail rd in requisitiondetails)
                {
                    if ((i.reqQty != 0) && (i.id == rd.Id))
                    {
                        var rul = await _clkService.findEmployeeByIdAsync(requisitions.Where(y => y.Id == rd.RequisitionId).FirstOrDefault().EmployeeId);
                        deptlist.Add(departments.Where(x => x.Id == rul.departmentId).FirstOrDefault());
                    }
                }
            }
            List<DisbursementList> disbursementList = new List<DisbursementList>();
            foreach (Department d in deptlist)
            {
                DisbursementList newDL = new DisbursementList();
                newDL.DepartmentId = d.Id;
                newDL.date = DateTime.Now;
                newDL.deliveryPoint = collectionpoints.Where(x => x.Id == d.Id).FirstOrDefault().collectionPoint;
                //d.Collection.collectionPoint;
                disbursementList.Add(newDL);
                _clkService.saveDisbursementList(newDL);

            }
            #endregion

            foreach (DisbursementList dl in disbursementList)
            {
                foreach (fakeRequisitionDetails i in fakeRequisitions)
                {
                    foreach (RequisitionDetail rd in requisitiondetails)
                    {
                        if ((i.reqQty != 0) && (i.id == rd.Id))
                        {
                            var currEmp = await _clkService.findEmployeeByIdAsync(requisitions.Where(y => y.Id == rd.RequisitionId).FirstOrDefault().EmployeeId);

                            if (currEmp.departmentId == dl.DepartmentId)
                            {
                                #region saving stockadjustments                               
                                StockAdjustmentDetail SAD = new StockAdjustmentDetail();
                                SAD.stockAdjustmentId = newSA.Id;
                                SAD.StationeryId = rd.StationeryId;
                                SAD.discpQty = -(i.reqQty);
                                SAD.comment = "sent to " + departments.Where(x => x.Id == currEmp.departmentId).FirstOrDefault().deptName;
                                SAD.Status = "Approved";
                                _clkService.saveStockAdjustmentDetail(SAD);
                                #endregion

                                #region updating stationeries item
                                foreach (Stationery s in stationeries)
                                {
                                    if (s.Id == rd.StationeryId)
                                    {
                                        s.inventoryQty -= i.reqQty;
                                        _clkService.updateStationery(s);
                                    }
                                }
                                #endregion

                                #region updating requisition detail

                                rd.rcvQty += i.reqQty;
                                Console.WriteLine($"{rd.Id},{rd.reqQty},{rd.rcvQty},{rd.StationeryId}, for the incoming {i.reqQty}");
                                _clkService.udpateRequisitionDetail(rd);
                                #endregion

                                #region creating disbursements
                                DisbursementDetail currDB = new DisbursementDetail();
                                currDB.DisbursementListId = dl.id;
                                currDB.qty = i.reqQty;
                                currDB.RequisitionDetailId = rd.Id;
                                _clkService.saveDisbursementDetail(currDB);
                                #endregion



                                Console.WriteLine("id:" + i.id + ", reqID:" + rd.RequisitionId + ", qty:" + i.reqQty);
                            }
                        }

                    }

                }
            }
            Console.WriteLine("done");
            return Ok(disbursementList);

        }



        //end

        #region InvCheck & ReceivedGoods
        [HttpPost("stkAd/{id}")]
        public async Task<ActionResult<StockAdjustment>> PostTestStkAd(
               [FromBody] List<StockAdjustmentDetail> stockAdjustmentDetails, int id)
        {
            StockAdjustment stkAdj = new StockAdjustment()
            {
                date = DateTime.Now,
                type = "inventory check",
                EmployeeId = id
            };

            var result = await _clkService.generateStkAdjustmentAsync(stkAdj, stockAdjustmentDetails); //SaveChangesAsync();
            if (result != null)
                return CreatedAtAction(
                    nameof(GetStkAdjId), new { id = result.Id }, result);
            else
                return NotFound("Sry failed.");
        }

        [HttpGet("stkAd/get/{id}")]
        public async Task<ActionResult<StockAdjustment>> GetStkAdjId(int id)
        {
            var result = await _clkService.findStockAdjustmentByIdAsync(id);
            if (result != null)
                //Docs says that Ok(...) will AUTO TRANSFER result into JSON Type
                return Ok(result);
            else
                //this help to return a NOTfOUND result, u can customerize the string.
                return NotFound("Suppliers not found");
        }
        [HttpPut("stkAd/put")]
        public Task<ActionResult<StockAdjustment>> PutStkAd([FromBody] List<StockAdjustmentDetail> stockAdjustmentDetails)
        {
            _clkService.updateStockAdjustment(stockAdjustmentDetails);
            return null;
        }
        [HttpPost("receivedGoods/{id}")]
        public async Task<ActionResult<StockAdjustment>> PostReceivedGoods(
               [FromBody] List<StockAdjustmentDetail> stockAdjustmentDetails, int id)
        {
            StockAdjustment stkAdj = new StockAdjustment()
            {
                date = DateTime.Now,
                type = "Received Goods",
                EmployeeId = id
            };

            var result = await _clkService.generateReceivedGoodsAsync(stkAdj, stockAdjustmentDetails); //SaveChangesAsync();
            if (result != null)
                return null;
            else
                return NotFound("Sry failed.");
        }
        #endregion
        #endregion

        #region Disbursement and Distribution
        [HttpGet("disbursements/{id}")]
        public async Task<ActionResult<List<DisbursementList>>> GetAllDisbursementList(int id)
        {
            var clerk = await _clkService.findEmployeeByIdAsync(id);
            var collectionPts = await _clkService.findAllCollectionPointAsync();
            var currCollectionPts = collectionPts.Where(x => x.clerkId == clerk.Id);
            var currDeliveryPoint = currCollectionPts.Select(x => x.collectionPoint);
            var allDL= await _clkService.findAllDisbursementListAsync();
            var result = allDL.Where(x => currDeliveryPoint.Contains(x.deliveryPoint));

            foreach (DisbursementList dl in result)
            {
                Console.WriteLine("id: " + dl.id + " dept id: " + dl.DepartmentId);
            }
            if (result != null)
            {
                return Ok(result);
            }
            else { return NotFound("nothing pending"); }
        }

        [HttpGet("departmentReps")]
        public async Task<ActionResult<List<Employee>>> GetAllDepartmentReps()
        {
            var allEmps = await _clkService.findEmployeesAsync();
            var departments = await _clkService.findAllDepartmentAsync();
            var deptReps = allEmps.Where(x => x.role.Equals("REPRESENTATIVE"));
            if (allEmps != null && departments != null)
            {
                foreach (Employee dr in deptReps)
                {
                    foreach (Department d in departments)
                    {
                        if (dr.departmentId == d.Id)
                        {
                            dr.email = d.deptName;
                        }
                    }
                }
            }
            return Ok(deptReps);
        }

        #endregion
        #region place order 
        /*[HttpGet("placeOrder")]
        public async Task<ActionResult<ReOrderRecViewModel>> GetReOrderRec()
        {
            //iterate through stationery
            var stationeries = await _clkService.findAllStationeriesAsync();
            IList<ReOrderRecViewModel> result = new List<ReOrderRecViewModel>();
            int id = 0;

            foreach (Stationery s in stationeries)
            {
                //find all suppliers that supply stationery 
                ICollection<Supplier> suppliers = await _clkService.findSupplierByStationeryId(1);
                //s.Id);

                //create a ReOrderRecViewModel for each stationery
                ReOrderRecViewModel reorder = new ReOrderRecViewModel()
                {
                    id = id,
                    stationery = s,
                    suppliers = suppliers
                };

                result.Add(reorder);
                id++;
            }

            if (result != null)
            {
                return Ok(result);
            }

            else
                return NotFound("No reorder items at this time.");
        }*/

        //api to get current clerk Id [HttpGet("/clerk")]

        [HttpPost("generatePO")]
        public int[] PostPurchaseOrder(
              [FromBody] List<PurchaseOrder> purchaseOrders)
        {
            int[] result = new int[purchaseOrders.Count];

            for (int i = 0; i < purchaseOrders.Count; i++)
            {
                PurchaseOrder po = purchaseOrders[i];
                po.dateOfOrder = DateTime.Now;
                _clkService.savePurchaseOrder(po);
                result[i] = po.id;
            }

            return result;
        }

        [HttpGet("getPO/{id}")]
        public async Task<ActionResult<List<PurchaseOrderDetail>>> GetPurchaseOrder(int id)
        {
            var result = await _clkService.findPOById(id);

            if (result != null)
                return Ok(result);
            else return null;

        }



        [HttpGet("getEmployee/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var result = await _clkService.findEmployeeByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
                return null;

        }

        [HttpGet("getSupplier/{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var result = await _clkService.findSupplierByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
                return null;


        }

        [HttpPost("PORecieved")]
        public async Task<ActionResult<PurchaseOrder>> PORecieved([FromBody] PurchaseOrder p)
        {
            PurchaseOrder po = await _clkService.findPOById(p.id);

            if (po != null)
            {
                po.status = "delivered";
                _clkService.updatePO(po);
                Console.WriteLine("PO Updated");
            }

            //return po;
            return Ok(p.id);
        }

        [HttpGet("getReorderItems")]
        public ActionResult<IList<Stationery>> getReorderItems()
        {
            #region  sql conn then sqlcommand
            string cnstr = "Server=tcp:team8-sa50.database.windows.net,1433;Initial Catalog=ADProj;Persist Security Info=False;User ID=Bianca;Password=!Str0ngPsword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=600;";
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();

            string sqlstr = "select ((((sum(reqDetail.reqQty) - sum(rcvQty))-sum(Stationery.inventoryQty))-sum(PoDetail.qty)) + sum(Stationery.reOrderLevel)) as difQty,Stationery.Id,Stationery.category,Stationery.[desc],Stationery.unit,Stationery.reOrderQty,Stationery.reOrderLevel,Stationery.inventoryQty " +
                "from RequisitionDetail_Table as reqDetail, Stationery_Table as Stationery, PurchaseOrderDetail_Table as PODetail, PurchaseOrder_Table as PO " +
                "where reqDetail.status != 'Delivered' and Stationery.Id = reqDetail.StationeryId and PO.status = 'ordered' and PODetail.PurchaseOrderId = PO.id and PODetail.StationeryId = Stationery.Id " +
                "group by Stationery.Id,Stationery.category,Stationery.[desc],Stationery.unit,Stationery.reOrderQty,Stationery.reOrderLevel,Stationery.inventoryQty";

            SqlCommand cmd = new SqlCommand(sqlstr, cn);

            SqlDataReader dr = cmd.ExecuteReader();
            IList<Stationery> result = new List<Stationery>();
            while (dr.Read())
            {
                if (int.Parse(dr["difQty"].ToString()) > 0)
                {
                    Stationery items = new Stationery()
                    {
                        Id = int.Parse(dr["Id"].ToString()),
                        reOrderQty = int.Parse(dr["difQty"].ToString()),
                        // category = dr["category"].ToString(),
                        desc = dr["desc"].ToString(),
                        unit = dr["unit"].ToString(),
                        //reOrderQty = int.Parse(dr["reOrderQty"].ToString()),
                        //reOrderLevel = int.Parse(dr["reOrderLevel"].ToString()),
                        //inventoryQty = int.Parse(dr["inventoryQty"].ToString()),
                    };
                    result.Add(items);
                }
            }
            dr.Close();
            cn.Close();

            foreach (Stationery rt in result)
            {
                Console.WriteLine(rt.ToString());//output testing
            }

            if (result != null)
                return Ok(result);
            else
                return NotFound("QUERY FAILED.");
        }

        [HttpGet("ItemsNeedOrder")]
        public async Task<ActionResult<List<Stationery>>> GetItemsNeedOrder()
        {
            //iterate through stationery
            IList<Stationery> stationeries = await _clkService.findAllStationeriesAsync();
            List<Stationery> itemsNeedOrder =
                stationeries.Where(x => x.inventoryQty < x.reOrderLevel).ToList();
            return itemsNeedOrder;

        }
        [HttpGet("getSupplierItems/{id}")]
        public IList<SupplierItem> GetSupplierItemsListByStationeryId(int id)
        {
            IList<SupplierItem> result = _clkService.findSuppliersByStationeryId(id);
            foreach (SupplierItem s in result)
            {
                Console.WriteLine(s.StationeryId);
            }
            if (result != null)
            {
                return result;
            }
            else
                return null;


        }

        [HttpGet("getPOD/{id}")]
        public IList<PurchaseOrderDetail> GetPurchaseOrderDetail(int id)
        {
            IList<PurchaseOrderDetail> result = _clkService.findPODById(id);

            if (result != null)
                return result;
            else return null;

        }

        [HttpGet("getAllPOs")]
        public async Task<ActionResult<IList<PurchaseOrder>>> GetAllPos()
        {
            IList<PurchaseOrder> result = await _clkService.findAllPOAsync();

            if (result != null)
                return Ok(result);
            else return null;


        }



        #endregion


    }
    #endregion
}