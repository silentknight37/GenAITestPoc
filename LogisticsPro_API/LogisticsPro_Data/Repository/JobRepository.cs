using LogisticsPro_Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LogisticsPro_Data.Repository
{
    public class JobRepository : IJobRepository
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public JobRepository(DB_LogisticsproContext dB_LogisticsproContext) {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        public async Task<List<JobCard>> GetJobs(bool isFirstLoad, string? jobCardCode, string? jobCardDescription, List<int>? customerId, DateTime? effectiveDateFrom, DateTime? effectiveDateTo, List<int>? statusIds)
        {
            int takeValue = 100000;
            if (isFirstLoad)
            {
                takeValue = 10;
            }
            var lpJobCardList = new List<JobCard>();
            if (customerId == null || customerId.Count == 0)
            {
                lpJobCardList.AddRange(await (from lpj in dB_LogisticsproContext.LpJobCard
                                           join lpc in dB_LogisticsproContext.LpMCustomer on lpj.CustomerId equals lpc.Id
                                           join s in dB_LogisticsproContext.LpMStatus on lpj.StatusId equals s.Id
                                           join u in dB_LogisticsproContext.LpMUser on lpj.CreatedBy equals u.Id
                                           where (jobCardCode == null || lpj.JobCardCode.Contains(jobCardCode)) &&
                                           (jobCardDescription == null || lpj.JobDescription.Contains(jobCardDescription)) &&
                                           (effectiveDateFrom == null || lpj.EffectiveDate >= effectiveDateFrom) &&
                                           (effectiveDateTo == null || lpj.EffectiveDate <= effectiveDateTo)
                                           select new JobCard
                                           {
                                               Id = lpj.Id,
                                               JobCardCode = lpj.JobCardCode,
                                               CustomerId = lpj.CustomerId,
                                               CustomerName = lpc.CustomerName,
                                               CustomerRef = lpj.CustomerRef,
                                               JobCardDescription = lpj.JobDescription,
                                               EffectiveDate = lpj.EffectiveDate,
                                               Remarks = lpj.Remarks,
                                               CreatedDate = lpj.CreatedDate,
                                               Status = s.StatusName,
                                               StatusId=lpj.StatusId,
                                               CreatedBy = u.UserName
                                           }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());

            }
            else
            {
                var filterLpJobCardList = new List<JobCard>();
                foreach (var cId in customerId)
                {
                    filterLpJobCardList.AddRange(await (from lpj in dB_LogisticsproContext.LpJobCard
                                                  join lpc in dB_LogisticsproContext.LpMCustomer on lpj.CustomerId equals lpc.Id
                                                  join s in dB_LogisticsproContext.LpMStatus on lpj.StatusId equals s.Id
                                                  join u in dB_LogisticsproContext.LpMUser on lpj.CreatedBy equals u.Id
                                                  where (jobCardCode == null || lpj.JobCardCode.Contains(jobCardCode)) &&
                                                  (jobCardDescription == null || lpj.JobDescription.Contains(jobCardDescription)) &&
                                                  ( lpj.CustomerId == cId) &&
                                                  (effectiveDateFrom == null || lpj.EffectiveDate >= effectiveDateFrom) &&
                                                  (effectiveDateTo == null || lpj.EffectiveDate <= effectiveDateTo)
                                                  select new JobCard
                                                  {
                                                      Id = lpj.Id,
                                                      JobCardCode = lpj.JobCardCode,
                                                      CustomerId = lpj.CustomerId,
                                                      CustomerName = lpc.CustomerName,
                                                      CustomerRef = lpj.CustomerRef,
                                                      JobCardDescription = lpj.JobDescription,
                                                      EffectiveDate = lpj.EffectiveDate,
                                                      Remarks = lpj.Remarks,
                                                      CreatedDate = lpj.CreatedDate,
                                                      Status = s.StatusName,
                                                      StatusId = lpj.StatusId,
                                                      CreatedBy = u.UserName
                                                  }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());
                }

                var orderList = filterLpJobCardList.OrderBy(i => i.EffectiveDate).ToList();
                lpJobCardList.AddRange(orderList);
            }
            var responseList = lpJobCardList.Where(i => statusIds == null || statusIds.Count == 0 || statusIds.Any(x => x == 0 || i.StatusId == x)).ToList();


            return responseList;
        }
        public async Task<int> SaveJob(JobSaveRequest jobSaveRequest)
        {
            try
            {
                if (jobSaveRequest.Id > 0)
                {
                    var lpJobCard = await dB_LogisticsproContext.LpJobCard.FirstOrDefaultAsync(i => i.Id == jobSaveRequest.Id);
                    if (lpJobCard != null)
                    {
                        lpJobCard.JobDescription = jobSaveRequest.JobDescription;
                        lpJobCard.CustomerId = jobSaveRequest.CustomerId;
                        lpJobCard.Remarks = jobSaveRequest.Remarks;
                        lpJobCard.EffectiveDate = jobSaveRequest.EffectiveDate;
                        lpJobCard.CustomerRef = jobSaveRequest.CustomerRef;

                        lpJobCard.UpdatedBy = jobSaveRequest.UserId;
                        lpJobCard.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJobCard).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJobCardAudit jobCardAuditUpdate = new LpJobCardAudit();
                        jobCardAuditUpdate.Id = lpJobCard.Id;
                        jobCardAuditUpdate.JobCardCode = lpJobCard.JobCardCode;
                        jobCardAuditUpdate.JobDescription = jobSaveRequest.JobDescription;
                        jobCardAuditUpdate.CustomerId = jobSaveRequest.CustomerId;
                        jobCardAuditUpdate.Remarks = jobSaveRequest.Remarks;
                        jobCardAuditUpdate.EffectiveDate = jobSaveRequest.EffectiveDate;
                        jobCardAuditUpdate.CustomerRef = jobSaveRequest.CustomerRef;
                        jobCardAuditUpdate.StatusId = lpJobCard.StatusId;
                        jobCardAuditUpdate.CreatedBy = lpJobCard.CreatedBy;
                        jobCardAuditUpdate.CreatedDate = lpJobCard.CreatedDate;
                        jobCardAuditUpdate.UpdatedBy = lpJobCard.CreatedBy;
                        jobCardAuditUpdate.UpdatedDate = lpJobCard.UpdatedDate;
                        jobCardAuditUpdate.AuditAction = "UPDATE";
                        jobCardAuditUpdate.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJobCardAudit.Add(jobCardAuditUpdate);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        return lpJobCard.Id;
                    }
                    return 0;
                }

                var jobPrefixCode = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o=>o.SystemCode== "JobPrefix");
                var currentJobCardNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o=>o.SystemCode== "CurrentJobCardNumber");
                string jobCardCode = string.Empty;
                int nextNumber = 0;

                if (jobPrefixCode != null && currentJobCardNumber != null) 
                {
                    nextNumber = int.Parse(currentJobCardNumber.SystemValue) + 1;
                    jobCardCode = $"{jobPrefixCode.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpJobCard jobCard = new LpJobCard();
                jobCard.JobCardCode = jobCardCode;
                jobCard.JobDescription = jobSaveRequest.JobDescription;
                jobCard.CustomerId = jobSaveRequest.CustomerId;
                jobCard.Remarks = jobSaveRequest.Remarks;
                jobCard.EffectiveDate = jobSaveRequest.EffectiveDate;
                jobCard.CustomerRef = jobSaveRequest.CustomerRef;
                jobCard.StatusId = 1;
                jobCard.CreatedBy = jobSaveRequest.UserId;
                jobCard.CreatedDate = DateTime.UtcNow;
                jobCard.UpdatedBy= jobSaveRequest.UserId;
                jobCard.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJobCard.Add(jobCard);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJobCardAudit jobCardAuditInsert = new LpJobCardAudit();
                jobCardAuditInsert.Id = jobCard.Id;
                jobCardAuditInsert.JobCardCode = jobCard.JobCardCode;
                jobCardAuditInsert.JobDescription = jobSaveRequest.JobDescription;
                jobCardAuditInsert.CustomerId = jobSaveRequest.CustomerId;
                jobCardAuditInsert.Remarks = jobSaveRequest.Remarks;
                jobCardAuditInsert.EffectiveDate = jobSaveRequest.EffectiveDate;
                jobCardAuditInsert.CustomerRef = jobSaveRequest.CustomerRef;
                jobCardAuditInsert.StatusId = jobCard.StatusId;
                jobCardAuditInsert.CreatedBy = jobCard.CreatedBy;
                jobCardAuditInsert.CreatedDate = jobCard.CreatedDate;
                jobCardAuditInsert.UpdatedBy = jobCard.UpdatedBy;
                jobCardAuditInsert.UpdatedDate = jobCard.UpdatedDate;
                jobCardAuditInsert.AuditAction = "INSERT";
                jobCardAuditInsert.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJobCardAudit.Add(jobCardAuditInsert);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentJobCardNumber.SystemValue = nextNumber.ToString();
                currentJobCardNumber.UpdatedDate = DateTime.UtcNow;
                currentJobCardNumber.UpdatedBy = jobSaveRequest.UserId;
                dB_LogisticsproContext.Entry(currentJobCardNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return jobCard.Id;
            }
            catch (Exception e)
            {

                throw;
                return 0;
            }
            return 0;

        }
        public async Task<bool> RemoveJobCard(int id)
        {
            try
            {
                var lpJobCard = await dB_LogisticsproContext.LpJobCard.FirstOrDefaultAsync(i => i.Id == id);
                dB_LogisticsproContext.LpJobCard.Remove(lpJobCard);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJobCardAudit jobCardAuditDelete = new LpJobCardAudit();
                jobCardAuditDelete.Id = lpJobCard.Id;
                jobCardAuditDelete.JobCardCode = lpJobCard.JobCardCode;
                jobCardAuditDelete.JobDescription = lpJobCard.JobDescription;
                jobCardAuditDelete.CustomerId = lpJobCard.CustomerId;
                jobCardAuditDelete.Remarks = lpJobCard.Remarks;
                jobCardAuditDelete.EffectiveDate = lpJobCard.EffectiveDate;
                jobCardAuditDelete.CustomerRef = lpJobCard.CustomerRef;
                jobCardAuditDelete.StatusId = lpJobCard.StatusId;
                jobCardAuditDelete.CreatedBy = lpJobCard.CreatedBy;
                jobCardAuditDelete.CreatedDate = lpJobCard.CreatedDate;
                jobCardAuditDelete.UpdatedBy = lpJobCard.UpdatedBy;
                jobCardAuditDelete.UpdatedDate = lpJobCard.UpdatedDate;
                jobCardAuditDelete.AuditAction = "DELETE";
                jobCardAuditDelete.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJobCardAudit.Add(jobCardAuditDelete);
                await dB_LogisticsproContext.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw;
            }

            return true;
        }

        public async Task<int> CloseJobCard(CloseJobCardRequest closeJobCardRequest)
        {
            try
            {
                var lpJobCard = await dB_LogisticsproContext.LpJobCard.FirstOrDefaultAsync(i => i.Id == closeJobCardRequest.Id);
                if (lpJobCard != null)
                {
                    lpJobCard.StatusId = 2;

                    lpJobCard.UpdatedBy = closeJobCardRequest.UserId;
                    lpJobCard.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.Entry(lpJobCard).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();

                    LpJobCardAudit jobCardAuditUpdate = new LpJobCardAudit();
                    jobCardAuditUpdate.Id = lpJobCard.Id;
                    jobCardAuditUpdate.JobCardCode = lpJobCard.JobCardCode;
                    jobCardAuditUpdate.JobDescription = lpJobCard.JobDescription;
                    jobCardAuditUpdate.CustomerId = lpJobCard.CustomerId;
                    jobCardAuditUpdate.Remarks = lpJobCard.Remarks;
                    jobCardAuditUpdate.EffectiveDate = lpJobCard.EffectiveDate;
                    jobCardAuditUpdate.CustomerRef = lpJobCard.CustomerRef;
                    jobCardAuditUpdate.StatusId = lpJobCard.StatusId;
                    jobCardAuditUpdate.CreatedBy = lpJobCard.CreatedBy;
                    jobCardAuditUpdate.CreatedDate = lpJobCard.CreatedDate;
                    jobCardAuditUpdate.UpdatedBy = lpJobCard.CreatedBy;
                    jobCardAuditUpdate.UpdatedDate = lpJobCard.UpdatedDate;
                    jobCardAuditUpdate.AuditAction = "UPDATE";
                    jobCardAuditUpdate.AuditDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpJobCardAudit.Add(jobCardAuditUpdate);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    return lpJobCard.Id;
                }
                return 0;
            }
            catch (Exception e)
            {

                throw;
                return 0;
            }
            return 0;

        }

        public async Task<int> OpenJobCard(OpenJobCardRequest openJobCardRequest)
        {
            try
            {
                var lpJobCard = await dB_LogisticsproContext.LpJobCard.FirstOrDefaultAsync(i => i.Id == openJobCardRequest.Id);
                if (lpJobCard != null)
                {
                    lpJobCard.StatusId = 1;

                    lpJobCard.UpdatedBy = openJobCardRequest.UserId;
                    lpJobCard.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.Entry(lpJobCard).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                    return lpJobCard.Id;
                }
                return 0;
            }
            catch (Exception e)
            {

                throw;
                return 0;
            }
            return 0;

        }

        public async Task<JobCard> GetJob(int jobCardId)
        {
            var lpJobCard = await (from lpj in dB_LogisticsproContext.LpJobCard
                                       join lpc in dB_LogisticsproContext.LpMCustomer on lpj.CustomerId equals lpc.Id
                                   join s in dB_LogisticsproContext.LpMStatus on lpj.StatusId equals s.Id
                                   join u in dB_LogisticsproContext.LpMUser on lpj.CreatedBy equals u.Id
                                   where lpj.Id == jobCardId
                                       select new JobCard
                                       {
                                           Id = lpj.Id,
                                           JobCardCode = lpj.JobCardCode,
                                           CustomerId = lpj.CustomerId,
                                           CustomerName = lpc.CustomerName,
                                           CustomerRef = lpj.CustomerRef,
                                           JobCardDescription = lpj.JobDescription,
                                           EffectiveDate = lpj.EffectiveDate,
                                           Remarks = lpj.Remarks,
                                           CreatedDate=lpj.CreatedDate,
                                           Status = s.StatusName,
                                           StatusId=lpj.StatusId,
                                           CreatedBy=u.UserName
                                       }).FirstOrDefaultAsync();



            return lpJobCard;
        }

        public async Task<List<JobCardTransportation>> GetJobTransportations(int jobCardId)
        {
            var lpJobTransportation = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                             join j in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals j.Id
                                   where j.Id == jobCardId
                                   select new JobCardTransportation
                                   {
                                       Id = lpt.Id,
                                       JobCardId=j.Id,
                                       JobCardNo=j.JobCardCode,
                                       JobCardStatusId=j.StatusId,
                                       TransportationCode= lpt.TransportationCode,              
                                       PickupLocation = lpt.PickupLocation,
                                       DropoffLocation = lpt.DropoffLocation,
                                       PickupTime = lpt.PickupTime,
                                       Adults = lpt.Adults,
                                       Children = lpt.Children,
                                       Infants = lpt.Infants,
                                       VehicleType = lpt.VehicleType,
                                       FlightNo = lpt.FlightNo,
                                       FlightTime = lpt.FlightTime,
                                       PaxName = lpt.PaxName,
                                       Remarks= lpt.Remarks,
                                       CostTaxAmount= lpt.CostTaxAmount,
                                       SellTaxAmount= lpt.SellTaxAmount,
                                       CostBaseAmount= lpt.CostBaseAmount,
                                       SellBaseAmount= lpt.SellBaseAmount,
                                       Parking = lpt.Parking,
                                       ParkingTaxAmount= lpt.ParkingTaxAmount,
                                       Water = lpt.Water,
                                       WaterTaxAmount= lpt.WaterTaxAmount,
                                       Extras = lpt.Extras,
                                       ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                       ParkingSell = lpt.ParkingSell,
                                       ParkingTaxAmountSell = lpt.ParkingTaxAmountSell,
                                       WaterSell = lpt.WaterSell,
                                       WaterTaxAmountSell = lpt.WaterTaxAmountSell,
                                       ExtrasSell = lpt.ExtrasSell,
                                       ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                       IsInvoiced= lpt.IsInvoiced,
                                       IsBatched= lpt.IsBatched,
                                   }).OrderBy(i=>i.PickupTime).ToListAsync();


            return lpJobTransportation;
        }

        public async Task<JobCardTransportation> GetJobTransportation(int id)
        {
            var lpJobTransportation = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                             where lpt.Id == id
                                             select new JobCardTransportation
                                             {
                                                 Id = lpt.Id,
                                                 TransportationCode = lpt.TransportationCode,
                                                 CustomerRef=lpt.CustomerRef,
                                                 Remarks= lpt.Remarks,
                                                 DropoffLocation = lpt.DropoffLocation,
                                                 CostBaseAmount=lpt.CostBaseAmount,
                                                 CostTaxAmount=lpt.CostTaxAmount,
                                                 IsVatIncludedSell =lpt.IsVatIncludedSell,
                                                 IsVatIncludedCost =lpt.IsVatIncludedCost,
                                                 SellBaseAmount =lpt.SellBaseAmount,
                                                 SellTaxAmount=lpt.SellTaxAmount,
                                                 JobCardId=lpt.JobCardId,
                                                 PickupLocation = lpt.PickupLocation,
                                                 PickupTime = lpt.PickupTime,
                                                 Adults = lpt.Adults,
                                                 Children = lpt.Children,
                                                 Infants = lpt.Infants,
                                                 VehicleType = lpt.VehicleType,
                                                 FlightNo = lpt.FlightNo,
                                                 FlightTime = lpt.FlightTime,
                                                 PaxName = lpt.PaxName,
                                                 Parking = lpt.Parking,
                                                 ParkingTaxAmount=lpt.ParkingTaxAmount,
                                                 Extras = lpt.Extras,
                                                 ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                                 Water = lpt.Water,
                                                 WaterTaxAmount=lpt.WaterTaxAmount,
                                                 ParkingSell = lpt.ParkingSell,
                                                 ParkingTaxAmountSell=lpt.ParkingTaxAmountSell,
                                                 ExtrasSell = lpt.ExtrasSell,
                                                 ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                                 WaterSell = lpt.WaterSell,
                                                 WaterTaxAmountSell=lpt.WaterTaxAmountSell,
                                                 IsInvoiced = lpt.IsInvoiced,
                                                 IsBatched = lpt.IsBatched,
                                             }).FirstOrDefaultAsync();


            return lpJobTransportation;
        }

        public async Task<List<JobCardTransportation>> GetCostTransportation(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            var lpJobTransportations = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                              join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                              (bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                              (customerId == null || jc.CustomerId == customerId) &&
                                              (clientRef == null || lpt.CustomerRef == clientRef) &&
                                              (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date >= dateFrom.Value.Date)) &&
                                              (dateTo == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date <= dateTo.Value.Date)) 
                                              select new JobCardTransportation
                                              {
                                                  Id = lpt.Id,
                                                  IsJobCardClosed=jc.StatusId==2,
                                                  TransportationCode = lpt.TransportationCode,
                                                  CustomerRef = lpt.CustomerRef,
                                                  Remarks = lpt.Remarks,
                                                  DropoffLocation = lpt.DropoffLocation,
                                                  CostBaseAmount = lpt.CostBaseAmount,
                                                  CostTaxAmount = lpt.CostTaxAmount,
                                                  IsVatIncludedSell = lpt.IsVatIncludedSell,
                                                  IsVatIncludedCost = lpt.IsVatIncludedCost,
                                                  SellBaseAmount = lpt.SellBaseAmount,
                                                  IsInvoiced=lpt.IsInvoiced,
                                                  IsBatched=lpt.IsBatched,
                                                  IsPaymentVouchered=lpt.IsPaymentVouchered,
                                                  SellTaxAmount = lpt.SellTaxAmount,
                                                  JobCardId = lpt.JobCardId,
                                                  PickupLocation = lpt.PickupLocation,
                                                  PickupTime = lpt.PickupTime,
                                                  Adults = lpt.Adults,
                                                  Children = lpt.Children,
                                                  Infants = lpt.Infants,
                                                  VehicleType = lpt.VehicleType,
                                                  FlightNo = lpt.FlightNo,
                                                  FlightTime = lpt.FlightTime,
                                                  Parking = lpt.Parking,
                                                  ParkingTaxAmount= lpt.ParkingTaxAmount,
                                                  Extras = lpt.Extras,
                                                  ExtrasTaxAmount= lpt.ExtrasTaxAmount,
                                                  Water = lpt.Water,
                                                  WaterTaxAmount= lpt.WaterTaxAmount,
                                                  ParkingSell = lpt.ParkingSell,
                                                  ParkingTaxAmountSell = lpt.ParkingTaxAmountSell,
                                                  ExtrasSell = lpt.ExtrasSell,
                                                  ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                                  WaterSell = lpt.WaterSell,
                                                  WaterTaxAmountSell = lpt.WaterTaxAmountSell,
                                                  PaxName = lpt.PaxName
                                              }).OrderBy(i => i.PickupTime).ToListAsync();

            if((batchNo!=null &&batchNo.Length> 0)|| (vendorId != null && vendorId > 0))
            {
                var lpJobTransportationFilter = (from lpj in lpJobTransportations
                                                 join bl in dB_LogisticsproContext.LpLBatchLineItem on lpj.Id equals bl.ContextId
                                                 join b in dB_LogisticsproContext.LpLBatch on bl.BatchId equals b.Id
                                                 where (batchNo == null || b.BatchCode.Contains(batchNo)) &&
                                                 (vendorId == null || b.VenderId == vendorId)
                                                 select lpj).ToList();

                return lpJobTransportationFilter;
            }

            return lpJobTransportations;
        }

        public async Task<List<JobCardHotel>> GetCostHotel(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            var lpJobHotels = await (from lph in dB_LogisticsproContext.LpJHotel
                                              join jc in dB_LogisticsproContext.LpJobCard on lph.JobCardId equals jc.Id
                                              join v in dB_LogisticsproContext.LpMVender on lph.VendorId equals v.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                              (bookingRef == null || lph.HotelCode.Contains(bookingRef)) &&
                                              (customerId == null || jc.CustomerId == customerId) &&
                                              (vendorId == null || lph.VendorId == vendorId) &&
                                              (dateFrom == null || (lph.CheckIn.HasValue && lph.CheckIn.Value.Date >= dateFrom.Value.Date)) &&
                                              (dateTo == null || (lph.CheckOut.HasValue && lph.CheckOut.Value.Date <= dateTo.Value.Date))
                                              select new JobCardHotel
                                              {
                                                  Id = lph.Id,
                                                  HotelCode = lph.HotelCode,
                                                  JobCardNo = jc.JobCardCode,
                                                  JobCardStatusId = jc.StatusId,
                                                  PaxName = lph.PaxName,
                                                  CheckIn = lph.CheckIn,
                                                  CheckOut = lph.CheckOut,
                                                  Adults = lph.Adults,
                                                  Children = lph.Children,
                                                  Infants = lph.Infants,
                                                  HotelName = lph.HotelName,
                                                  HotelAddress1 = lph.HotelAddress1,
                                                  HotelAddress2 = lph.HotelAddress2,
                                                  CostBaseAmount = lph.CostBaseAmount,
                                                  CostTaxAmount = lph.CostTaxAmount,
                                                  SellBaseAmount = lph.SellBaseAmount,
                                                  SellTaxAmount = lph.SellTaxAmount,
                                                  VenderName = v.VenderName,
                                                  CountryCode = v.CountryCode,
                                                  City = v.City,
                                                  VenderId = v.Id,
                                                  VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                                  VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                                  Remarks = lph.Remarks,
                                                  RoomType = lph.RoomType,
                                                  HotelConfirmation = lph.HotelConfirmation,
                                                  IsInvoiced = lph.IsInvoiced,
                                              }).OrderBy(i => i.CheckIn).ToListAsync();

            

            return lpJobHotels;
        }

        public async Task<List<JobCardVisa>> GetCostVisa(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            var lpJobVisas = await (from lpv in dB_LogisticsproContext.LpJVisa
                                              join jc in dB_LogisticsproContext.LpJobCard on lpv.JobCardId equals jc.Id
                                              join v in dB_LogisticsproContext.LpMVender on lpv.VendorId equals v.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                              (bookingRef == null || lpv.VisaCode.Contains(bookingRef)) &&
                                              (customerId == null || jc.CustomerId == customerId) &&
                                              (vendorId == null || lpv.VendorId == vendorId) &&
                                              (dateFrom == null || (lpv.CreatedDate.HasValue && lpv.CreatedDate.Value.Date >= dateFrom.Value.Date)) &&
                                              (dateTo == null || (lpv.CreatedDate.HasValue && lpv.CreatedDate.Value.Date <= dateTo.Value.Date))
                                              select new JobCardVisa
                                              {
                                                  Id = lpv.Id,
                                                  VisaCode = lpv.VisaCode,
                                                  VendorName = v.VenderName,
                                                  JobCardNo = jc.JobCardCode,
                                                  JobCardStatusId = jc.StatusId,
                                                  VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                                  VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                                  VendorId = v.Id,
                                                  PaxName = lpv.PaxName,
                                                  PassportNo = lpv.PassportNo,
                                                  IsVatIncludedCost = lpv.IsVatIncludedCost,
                                                  IsVatIncludedSell = lpv.IsVatIncludedSell,
                                                  CostBaseAmount = lpv.CostBaseAmount,
                                                  CostTaxAmount = lpv.CostTaxAmount,
                                                  SellBaseAmount = lpv.SellBaseAmount,
                                                  SellTaxAmount = lpv.SellTaxAmount,
                                                  JobCardId = lpv.JobCardId,
                                                  Nationality = lpv.Nationality,
                                                  Remarks = lpv.Remarks,
                                                  IsInvoiced = lpv.IsInvoiced,
                                                  CreatedDate = lpv.CreatedDate
                                              }).OrderBy(i => i.CreatedDate).ToListAsync();

            return lpJobVisas;
        }

        public async Task<List<JobCardMiscellaneous>> GetCostMiscellaneous(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            var lpJMiscellaneous = await (from lpm in dB_LogisticsproContext.LpJMiscellaneous
                                              join jc in dB_LogisticsproContext.LpJobCard on lpm.JobCardId equals jc.Id
                                              join v in dB_LogisticsproContext.LpMVender on lpm.VendorId equals v.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                              (bookingRef == null || lpm.MiscellaneousCode.Contains(bookingRef)) &&
                                              (customerId == null || jc.CustomerId == customerId) &&
                                              (vendorId == null || lpm.VendorId == vendorId) &&
                                              (dateFrom == null || (lpm.MisDate.HasValue && lpm.MisDate.Value.Date >= dateFrom.Value.Date)) &&
                                              (dateTo == null || (lpm.MisDate.HasValue && lpm.MisDate.Value.Date <= dateTo.Value.Date))
                                              select new JobCardMiscellaneous
                                              {
                                                  Id = lpm.Id,
                                                  MiscellaneousCode = lpm.MiscellaneousCode,
                                                  PaxName = lpm.PaxName,
                                                  VendorName = v.VenderName,
                                                  JobCardNo = jc.JobCardCode,
                                                  JobCardStatusId = jc.StatusId,
                                                  VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                                  VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                                  VendorId = v.Id,
                                                  PaxNumber = lpm.PaxNumber,
                                                  Description = lpm.Description,
                                                  MisDate = lpm.MisDate,
                                                  IsVatIncludedCost = lpm.IsVatIncludedCost,
                                                  IsVatIncludedSell = lpm.IsVatIncludedSell,
                                                  CostBaseAmount = lpm.CostBaseAmount,
                                                  CostTaxAmount = lpm.CostTaxAmount,
                                                  SellBaseAmount = lpm.SellBaseAmount,
                                                  SellTaxAmount = lpm.SellTaxAmount,
                                                  JobCardId = lpm.JobCardId,
                                                  Remarks = lpm.Remarks,
                                                  IsInvoiced = lpm.IsInvoiced,
                                                  IsFinance = lpm.IsFinance
                                              }).OrderBy(i => i.MisDate).ToListAsync();

            
            return lpJMiscellaneous;
        }

        public async Task<List<JobCardHotel>> GetJobHotels(int jobCardId)
        {
            var lpJobHotels = await (from lph in dB_LogisticsproContext.LpJHotel
                                     join j in dB_LogisticsproContext.LpJobCard on lph.JobCardId equals j.Id
                                     join v in dB_LogisticsproContext.LpMVender on lph.VendorId equals v.Id
                                     where lph.JobCardId == jobCardId
                                             select new JobCardHotel
                                             {
                                                 Id = lph.Id,
                                                 HotelCode = lph.HotelCode,
                                                 JobCardNo = j.JobCardCode,
                                                 JobCardStatusId=j.StatusId,
                                                 PaxName =lph.PaxName,
                                                 CheckIn = lph.CheckIn,
                                                 CheckOut = lph.CheckOut,
                                                 Adults = lph   .Adults,
                                                 Children = lph.Children,
                                                 Infants = lph.Infants,
                                                 HotelName = lph.HotelName,
                                                 HotelAddress1 = lph.HotelAddress1,
                                                 HotelAddress2 = lph.HotelAddress2,
                                                 CostBaseAmount = lph.CostBaseAmount,
                                                 CostTaxAmount = lph.CostTaxAmount,
                                                 SellBaseAmount = lph.SellBaseAmount,
                                                 SellTaxAmount = lph.SellTaxAmount,
                                                 VenderName = v.VenderName,
                                                 CountryCode= v.CountryCode,
                                                 City=v.City,
                                                 VenderId = v.Id,
                                                 VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                                 VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                                 Remarks = lph.Remarks,
                                                 RoomType= lph.RoomType,
                                                 HotelConfirmation=lph.HotelConfirmation,
                                                 IsInvoiced = lph.IsInvoiced,
                                             }).OrderBy(i => i.CheckIn).ToListAsync();

            

            return lpJobHotels;
        }
        public async Task<JobCardHotel> GetJobHotel(int id)
        {
           
            var lpJobHotel = await (from lph in dB_LogisticsproContext.LpJHotel
                                    join j in dB_LogisticsproContext.LpJobCard on lph.JobCardId equals j.Id
                                    join v in dB_LogisticsproContext.LpMVender on lph.VendorId equals v.Id
                                     where lph.Id == id
                                     select new JobCardHotel
                                     {
                                         Id = lph.Id,
                                         HotelCode = lph.HotelCode,
                                         JobCardNo=j.JobCardCode,
                                         PaxName = lph.PaxName,
                                         CheckIn = lph.CheckIn,
                                         CheckOut = lph.CheckOut,
                                         Adults = lph.Adults,
                                         Children = lph.Children,
                                         Infants = lph.Infants,
                                         HotelName = lph.HotelName,
                                         HotelAddress1=lph.HotelAddress1,
                                         HotelAddress2=lph.HotelAddress2,
                                         VenderName = v.VenderName,
                                         VenderId =v.Id,
                                         IsVatIncludedCost=lph.IsVatIncludedCost,
                                         IsVatIncludedSell=lph.IsVatIncludedSell,
                                         CostBaseAmount = lph.CostBaseAmount,
                                         CostTaxAmount = lph.CostTaxAmount,
                                         SellBaseAmount = lph.SellBaseAmount,
                                         SellTaxAmount = lph.SellTaxAmount,
                                         JobCardId = lph.JobCardId,
                                         Remarks=lph.Remarks,
                                         HotelConfirmation=lph.HotelConfirmation,
                                         RoomType=lph.RoomType,
                                         IsInvoiced = lph.IsInvoiced,
                                     }).FirstOrDefaultAsync();

            
            return lpJobHotel;
        }

        public async Task<List<JobCardVisa>> GetJobVisas(int jobCardId)
        {
                var lpJobVisa = await (from lpv in dB_LogisticsproContext.LpJVisa
                                       join j in dB_LogisticsproContext.LpJobCard on lpv.JobCardId equals j.Id
                                       join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                       join v in dB_LogisticsproContext.LpMVender on lpv.VendorId equals v.Id
                                       where lpv.JobCardId == jobCardId
                                       select new JobCardVisa
                                       {
                                           Id = lpv.Id,
                                           VisaCode = lpv.VisaCode,
                                           VendorName = v.VenderName,
                                           JobCardNo = j.JobCardCode,
                                           JobCardStatusId = j.StatusId,
                                           VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                           VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                           VendorId = v.Id,
                                           PaxName = lpv.PaxName,
                                           PassportNo = lpv.PassportNo,
                                           VisaTypeName = lpvt.VisaTypeName,
                                           IsVatIncludedCost = lpv.IsVatIncludedCost,
                                           IsVatIncludedSell = lpv.IsVatIncludedSell,
                                           CostBaseAmount = lpv.CostBaseAmount,
                                           CostTaxAmount = lpv.CostTaxAmount,
                                           SellBaseAmount = lpv.SellBaseAmount,
                                           SellTaxAmount = lpv.SellTaxAmount,
                                           JobCardId = lpv.JobCardId,
                                           Nationality = lpv.Nationality,
                                           Remarks = lpv.Remarks,
                                           IsInvoiced = lpv.IsInvoiced,
                                           CreatedDate=lpv.CreatedDate
                                       }).OrderBy(i => i.CreatedDate).ToListAsync();

                return lpJobVisa;
        }

        public async Task<JobCardVisa> GetJobVisa(int id)
        {
            var lpJobVisa = await (from lpv in dB_LogisticsproContext.LpJVisa
                                   join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                   join v in dB_LogisticsproContext.LpMVender on lpv.VendorId equals v.Id
                                   where lpv.Id == id
                                   select new JobCardVisa
                                   {
                                       Id = lpv.Id,
                                       VisaCode = lpv.VisaCode,
                                       VisaTypeId=lpv.VisaTypeId,
                                       VendorName = v.VenderName,
                                       VendorId = v.Id,
                                       PaxName = lpv.PaxName,
                                       PassportNo = lpv.PassportNo,
                                       VisaTypeName = lpvt.VisaTypeName,
                                       IsVatIncludedCost = lpv.IsVatIncludedCost,
                                       IsVatIncludedSell = lpv.IsVatIncludedSell,
                                       CostBaseAmount = lpv.CostBaseAmount,
                                       CostTaxAmount = lpv.CostTaxAmount,
                                       SellBaseAmount = lpv.SellBaseAmount,
                                       SellTaxAmount = lpv.SellTaxAmount,
                                       JobCardId = lpv.JobCardId,
                                       Nationality=lpv.Nationality,
                                       Remarks = lpv.Remarks,
                                       IsInvoiced = lpv.IsInvoiced,
                                   }).FirstOrDefaultAsync();


            return lpJobVisa;
        }

        public async Task<List<JobCardMiscellaneous>> GetJobMiscellanea(int jobCardId,int userId)
        {
            var lpJobMiscellaneous = await (from lpm in dB_LogisticsproContext.LpJMiscellaneous
                                            join j in dB_LogisticsproContext.LpJobCard on lpm.JobCardId equals j.Id
                                            join v in dB_LogisticsproContext.LpMVender on lpm.VendorId equals v.Id
                                            where lpm.JobCardId == jobCardId
                                             select new JobCardMiscellaneous
                                             {
                                                 Id = lpm.Id,
                                                 MiscellaneousCode = lpm.MiscellaneousCode,
                                                 PaxName = lpm.PaxName,
                                                 VendorName = v.VenderName,
                                                 JobCardNo = j.JobCardCode,
                                                 JobCardStatusId = j.StatusId,
                                                 VenderAddress1 = v.Address1 == null || v.Address1 == "" ? v.Address2 : $"{v.Address1}, {v.Address2} ",
                                                 VenderAddress2 = v.City == null || v.City == "" ? v.CountryCode : $"{v.City}, {v.CountryCode} ",
                                                 VendorId = v.Id,
                                                 PaxNumber = lpm.PaxNumber,
                                                 Description = lpm.Description,
                                                 MisDate= lpm.MisDate,
                                                 IsVatIncludedCost = lpm.IsVatIncludedCost,
                                                 IsVatIncludedSell = lpm.IsVatIncludedSell,
                                                 CostBaseAmount = lpm.CostBaseAmount,
                                                 CostTaxAmount = lpm.CostTaxAmount,
                                                 SellBaseAmount = lpm.SellBaseAmount,
                                                 SellTaxAmount = lpm.SellTaxAmount,
                                                 JobCardId = lpm.JobCardId,
                                                 Remarks = lpm.Remarks,
                                                 IsInvoiced = lpm.IsInvoiced,
                                                 IsFinance=lpm.IsFinance
                                             }).OrderBy(i => i.MisDate).ToListAsync();

            var user = await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(i => i.Id == userId);
            if (user != null && user.RoleId==(int)EnumRole.OperationClark)
            {
                return lpJobMiscellaneous.Where(i => i.IsFinance == false).ToList();
            }

            return lpJobMiscellaneous;
        }

        public async Task<JobCardMiscellaneous> GetJobMiscellaneous(int id,int userId)
        {
            var lpJobMiscellaneous = await (from lpm in dB_LogisticsproContext.LpJMiscellaneous
                                            join v in dB_LogisticsproContext.LpMVender on lpm.VendorId equals v.Id
                                            where lpm.Id == id
                                            select new JobCardMiscellaneous
                                            {
                                                Id = lpm.Id,
                                                MiscellaneousCode = lpm.MiscellaneousCode,
                                                VendorName = v.VenderName,
                                                VendorId = v.Id,
                                                PaxName = lpm.PaxName,
                                                PaxNumber = lpm.PaxNumber,
                                                MisDate =lpm.MisDate,
                                                IsVatIncludedCost = lpm.IsVatIncludedCost,
                                                IsVatIncludedSell = lpm.IsVatIncludedSell,
                                                CostBaseAmount = lpm.CostBaseAmount,
                                                CostTaxAmount = lpm.CostTaxAmount,
                                                SellBaseAmount = lpm.SellBaseAmount,
                                                SellTaxAmount = lpm.SellTaxAmount,
                                                JobCardId = lpm.JobCardId,
                                                Description = lpm.Description,
                                                Remarks = lpm.Remarks,
                                                IsInvoiced = lpm.IsInvoiced,
                                                IsFinance=lpm.IsFinance
                                            }).FirstOrDefaultAsync();


            return lpJobMiscellaneous;
        }

        public async Task<JobCardFinance> GetJobFinancRreceiptAndPayments(int jobCardId)
        {
            var jobCardFinance = new JobCardFinance();
            var lpJobFinanceReceiptList= new List<JobCardFinanceRreceipt>();
            var lpJobFinanceVoucherList = new List<JobCardFinancePaymentVoucher>();
            var lpJobFinanceInvoiceList = new List<JobCardFinanceInvoice>();

            var jobCardLineItems = new List<JobCardLineItem>();
            jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJTransportation.Where(i=>i.JobCardId== jobCardId).Select(x=>new JobCardLineItem
            {
                Id = x.Id,
                JobCardId = x.JobCardId,
                BookingRef = x.TransportationCode,
                CostBaseAmount = x.CostBaseAmount,
                CostTaxAmount = x.CostTaxAmount,
                Extras = x.Extras,
                ExtrasTaxAmount = x.ExtrasTaxAmount,
                Parking = x.Parking,
                Water = x.Water,
                ContextIdType = (int)EnumContextType.Transpotation
            }).ToListAsync());

            jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJHotel.Where(i => i.JobCardId == jobCardId).Select(x => new JobCardLineItem
            {
                Id = x.Id,
                JobCardId = x.JobCardId,
                BookingRef = x.HotelCode,
                CostBaseAmount = x.CostBaseAmount,
                CostTaxAmount = x.CostTaxAmount,
                ContextIdType = (int)EnumContextType.Hotel
            }).ToListAsync());

            jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJVisa.Where(i => i.JobCardId == jobCardId).Select(x => new JobCardLineItem
            {
                Id = x.Id,
                JobCardId = x.JobCardId,
                BookingRef = x.VisaCode,
                CostBaseAmount = x.CostBaseAmount,
                CostTaxAmount = x.CostTaxAmount,
                ContextIdType = (int)EnumContextType.Visa
            }).ToListAsync());

            jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJMiscellaneous.Where(i => i.JobCardId == jobCardId).Select(x => new JobCardLineItem
            {
                Id = x.Id,
                JobCardId = x.JobCardId,
                BookingRef = x.MiscellaneousCode,
                CostBaseAmount = x.CostBaseAmount,
                CostTaxAmount = x.CostTaxAmount,
                ContextIdType=(int)EnumContextType.Miscellaneous
            }).ToListAsync());



            var lpJobFinanceProformaReceipts = await (from r in dB_LogisticsproContext.LpFProformaInvoiceReceipt
                                                      join i in dB_LogisticsproContext.LpFProformaInvoice on r.ProformaInvoiceId equals i.Id
                                                      where i.JobCardId == jobCardId && i.StatusId!=3
                                                      select new JobCardFinanceRreceipt
                                                      {
                                                          Id = r.Id,
                                                          Amount = r.Amount,
                                                          Remarks = r.Remark,
                                                          ReceiptDate = r.PaymentDate,
                                                          ReceiptCode = r.ReceiptCode,
                                                          InvoiceNo = i.InvoiceCode
                                                      }).Distinct().ToListAsync();

            jobCardFinance.JobCardFinanceProformaInvoicesReceipt.AddRange(lpJobFinanceProformaReceipts);

            var lpJobFinanceProformaInvoice = await (from i in dB_LogisticsproContext.LpFProformaInvoice
                                             where i.JobCardId == jobCardId && i.StatusId != 3
                                                     select new JobCardFinanceInvoice
                                             {
                                                 Id = i.Id,
                                                 Amount = i.InvoiceAmount,
                                                 Remarks = i.Description,
                                                 InvoiceCode = i.InvoiceCode,
                                                 InvoiceDate = i.InvoiceDate,
                                                 InvoiceDueDate = i.InvoiceDueDate
                                             }).ToListAsync();

            jobCardFinance.JobCardFinanceProformaInvoices.AddRange(lpJobFinanceProformaInvoice);

            foreach (var jobCardLineItem in jobCardLineItems)
            {
                var lpJobFinanceReceipts = await (from r in dB_LogisticsproContext.LpFReceipt
                                                  join i in dB_LogisticsproContext.LpFInvoice on r.InvoiceId equals i.Id
                                                  join ra in dB_LogisticsproContext.LpFReceiptAllocation on r.Id equals ra.ReceiptId
                                                  where ra.ContextId== jobCardLineItem.Id && ra.ContextTypeId== jobCardLineItem.ContextIdType && i.StatusId != 3
                                                  && r.IsVoid==false
                                                  select new JobCardFinanceRreceipt
                                                  {
                                                      Id = r.Id,
                                                      Amount = r.Amount,
                                                      Remarks = r.Remark,
                                                      ReceiptDate = r.PaymentDate,
                                                      ReceiptCode= r.ReceiptCode,
                                                      InvoiceNo=i.InvoiceCode
                                                  }).Distinct().ToListAsync();
                
                lpJobFinanceReceiptList.AddRange(lpJobFinanceReceipts);


                 var lpJobFinanceVouchers = await (from pv in dB_LogisticsproContext.LpFPaymentVoucher
                                                  join v in dB_LogisticsproContext.LpMVender on pv.VendorId equals v.Id
                                                  join pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem on pv.Id equals pvi.PaymentVoucherId
                                                  where pvi.ContextId == jobCardLineItem.Id && pvi.ContextTypeId == jobCardLineItem.ContextIdType
                                                   select new JobCardFinancePaymentVoucher
                                                  {
                                                      Id=pv.Id,
                                                      Amount= jobCardLineItem.TotalCostPrice,
                                                      Remarks=pv.Remark,
                                                      VendorName=v.VenderName,
                                                      VoucherCode=pv.PaymentVoucherCode,
                                                      VoucherDate=pv.VoucherDate
                                                  }).ToListAsync();

                lpJobFinanceVoucherList.AddRange(lpJobFinanceVouchers);

                var lpJobFinanceInvoice = await (from i in dB_LogisticsproContext.LpFInvoice
                                                 join il in dB_LogisticsproContext.LpFInvoiceLineItem on i.Id equals il.InvoiceId
                                                 where il.ContextId == jobCardLineItem.Id && il.ContextTypeId == jobCardLineItem.ContextIdType && i.StatusId != 3
                                                 select new JobCardFinanceInvoice
                                                 {
                                                     Id = i.Id,
                                                     Amount = i.InvoiceAmount,
                                                     LineItemSellAmount = il.SellBaseAmount,
                                                     LineItemTaxAmount =il.SellTaxAmount,
                                                     Remarks = i.Remark,
                                                     InvoiceCode = i.InvoiceCode,
                                                     InvoiceDate = i.InvoiceDate,
                                                     InvoiceDueDate = i.InvoiceDueDate
                                                 }).ToListAsync();

                lpJobFinanceInvoiceList.AddRange(lpJobFinanceInvoice);
            }

            var jobFinanceReceipts = (from p in lpJobFinanceReceiptList
                                      group p by new { p.Id, p.ReceiptCode, p.Amount, p.Remarks, p.ReceiptDate, p.InvoiceNo } into g
                                      select new JobCardFinanceRreceipt
                                      {
                                          Id = g.Key.Id,
                                          Amount = g.Key.Amount,
                                          Remarks = g.Key.Remarks,
                                          ReceiptDate = g.Key.ReceiptDate,
                                          ReceiptCode = g.Key.ReceiptCode,
                                          InvoiceNo = g.Key.InvoiceNo
                                      }).ToList();

            jobCardFinance.JobCardFinanceRreceipt.AddRange(jobFinanceReceipts);

            var jobFinanceInvoices = (from p in lpJobFinanceInvoiceList
                                   group p by new { p.Id, p.InvoiceCode, p.Amount, p.Remarks, p.InvoiceDate,p.InvoiceDueDate } into g
                                   select new JobCardFinanceInvoice
                                   {
                                       Id = g.Key.Id,
                                       Amount = g.Key.Amount,
                                       Remarks = g.Key.Remarks,
                                       LineItemTotalAmount=g.Sum(x => x.LineItemAmount),
                                       InvoiceDate = g.Key.InvoiceDate,
                                       InvoiceDueDate = g.Key.InvoiceDueDate,
                                       InvoiceCode = g.Key.InvoiceCode
                                   }).ToList();

            jobCardFinance.JobCardFinanceInvoices.AddRange(jobFinanceInvoices);

            var paymentVouchers = (from p in lpJobFinanceVoucherList
                                   group p by new { p.Id, p.Remarks, p.VendorName, p.VoucherDate, p.VoucherCode } into g
                                   select new JobCardFinancePaymentVoucher
                                   {
                                       Id = g.Key.Id,
                                       Amount = g.Sum(i => i.Amount),
                                       Remarks = g.Key.Remarks,
                                       VendorName = g.Key.VendorName,
                                       VoucherCode = g.Key.VoucherCode,
                                       VoucherDate = g.Key.VoucherDate
                                   }).ToList();
            jobCardFinance.JobCardFinancePaymentVouchers.AddRange(paymentVouchers);

            return jobCardFinance;
        }
        public async Task<bool> SaveJobTransportation(JobCardTransportationRequest jobCardTransportationRequest)
        {
            try
            {
                if (jobCardTransportationRequest.Id > 0)
                {
                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == jobCardTransportationRequest.Id);
                    if (lpJTransportation != null)
                    {
                        lpJTransportation.PaxName = jobCardTransportationRequest.PaxName;
                        lpJTransportation.CustomerRef = jobCardTransportationRequest.CustomerRef;
                        lpJTransportation.Remarks = jobCardTransportationRequest.Remarks;
                        lpJTransportation.Adults=jobCardTransportationRequest.Adults;
                        lpJTransportation.Children = jobCardTransportationRequest.Children;
                        lpJTransportation.Infants = jobCardTransportationRequest.Infants;
                        lpJTransportation.CostBaseAmount = jobCardTransportationRequest.CostBaseAmount;
                        lpJTransportation.CostTaxAmount = jobCardTransportationRequest.CostTaxAmount;
                        lpJTransportation.SellBaseAmount= jobCardTransportationRequest.SellBaseAmount;
                        lpJTransportation.SellTaxAmount = jobCardTransportationRequest.SellTaxAmount;
                        lpJTransportation.DropoffLocation= jobCardTransportationRequest.DropoffLocation;
                        lpJTransportation.PickupLocation= jobCardTransportationRequest.PickupLocation;
                        lpJTransportation.PickupTime = jobCardTransportationRequest.PickupTime;
                        lpJTransportation.FlightNo = jobCardTransportationRequest.FlightNo;
                        lpJTransportation.FlightTime = jobCardTransportationRequest.FlightTime;
                        lpJTransportation.VehicleType = jobCardTransportationRequest.VehicleType;
                        lpJTransportation.IsVatIncludedCost = jobCardTransportationRequest.IsVatIncludedCost;
                        lpJTransportation.IsVatIncludedSell = jobCardTransportationRequest.IsVatIncludedSell;
                        lpJTransportation.Parking = jobCardTransportationRequest.Parking;
                        lpJTransportation.ParkingTaxAmount = jobCardTransportationRequest.ParkingTaxAmount;
                        lpJTransportation.Water = jobCardTransportationRequest.Water;
                        lpJTransportation.WaterTaxAmount = jobCardTransportationRequest.WaterTaxAmount;
                        lpJTransportation.Extras = jobCardTransportationRequest.Extras;
                        lpJTransportation.ExtrasTaxAmount = jobCardTransportationRequest.ExtrasTaxAmount;
                        lpJTransportation.ParkingSell = jobCardTransportationRequest.ParkingSell;
                        lpJTransportation.ParkingTaxAmountSell = jobCardTransportationRequest.ParkingTaxAmountSell;
                        lpJTransportation.WaterSell = jobCardTransportationRequest.WaterSell;
                        lpJTransportation.WaterTaxAmountSell = jobCardTransportationRequest.WaterTaxAmountSell;
                        lpJTransportation.ExtrasSell = jobCardTransportationRequest.ExtrasSell;
                        lpJTransportation.ExtrasTaxAmountSell = jobCardTransportationRequest.ExtrasTaxAmountSell;
                        lpJTransportation.UpdatedBy = jobCardTransportationRequest.UserId;
                        lpJTransportation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJTransportation).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJTransportationAudit transportationUpdateAudit = new LpJTransportationAudit();
                        transportationUpdateAudit.Id = lpJTransportation.Id;
                        transportationUpdateAudit.TransportationCode = lpJTransportation.TransportationCode;
                        transportationUpdateAudit.JobCardId = lpJTransportation.JobCardId;
                        transportationUpdateAudit.PaxName = lpJTransportation.PaxName;
                        transportationUpdateAudit.CustomerRef = lpJTransportation.CustomerRef;
                        transportationUpdateAudit.Remarks = lpJTransportation.Remarks;
                        transportationUpdateAudit.Adults = lpJTransportation.Adults;
                        transportationUpdateAudit.Children = lpJTransportation.Children;
                        transportationUpdateAudit.Infants = lpJTransportation.Infants;
                        transportationUpdateAudit.CostBaseAmount = lpJTransportation.CostBaseAmount;
                        transportationUpdateAudit.CostTaxAmount = lpJTransportation.CostTaxAmount;
                        transportationUpdateAudit.SellBaseAmount = lpJTransportation.SellBaseAmount;
                        transportationUpdateAudit.SellTaxAmount = lpJTransportation.SellTaxAmount;
                        transportationUpdateAudit.DropoffLocation = lpJTransportation.DropoffLocation;
                        transportationUpdateAudit.PickupLocation = lpJTransportation.PickupLocation;
                        transportationUpdateAudit.PickupTime = lpJTransportation.PickupTime;
                        transportationUpdateAudit.FlightNo = lpJTransportation.FlightNo;
                        transportationUpdateAudit.FlightTime = lpJTransportation.FlightTime;
                        transportationUpdateAudit.VehicleType = lpJTransportation.VehicleType;
                        transportationUpdateAudit.IsVatIncludedCost = lpJTransportation.IsVatIncludedCost;
                        transportationUpdateAudit.IsVatIncludedSell = lpJTransportation.IsVatIncludedSell;
                        transportationUpdateAudit.Extras = lpJTransportation.Extras;
                        transportationUpdateAudit.ExtrasTaxAmount = lpJTransportation.ExtrasTaxAmount;
                        transportationUpdateAudit.Parking = lpJTransportation.Parking;
                        transportationUpdateAudit.ParkingTaxAmount = lpJTransportation.ParkingTaxAmount;
                        transportationUpdateAudit.Water = lpJTransportation.Water;
                        transportationUpdateAudit.WaterTaxAmount = lpJTransportation.WaterTaxAmount;
                        transportationUpdateAudit.ExtrasSell = lpJTransportation.ExtrasSell;
                        transportationUpdateAudit.ExtrasTaxAmountSell = lpJTransportation.ExtrasTaxAmountSell;
                        transportationUpdateAudit.ParkingSell = lpJTransportation.ParkingSell;
                        transportationUpdateAudit.ParkingTaxAmountSell = lpJTransportation.ParkingTaxAmountSell;
                        transportationUpdateAudit.WaterSell = lpJTransportation.WaterSell;
                        transportationUpdateAudit.WaterTaxAmountSell = lpJTransportation.WaterTaxAmountSell;
                        transportationUpdateAudit.CreatedBy = lpJTransportation.CreatedBy;
                        transportationUpdateAudit.CreatedDate = lpJTransportation.CreatedDate;
                        transportationUpdateAudit.UpdatedBy = lpJTransportation.UpdatedBy;
                        transportationUpdateAudit.UpdatedDate = lpJTransportation.UpdatedDate;
                        transportationUpdateAudit.IsBatched = lpJTransportation.IsBatched;
                        transportationUpdateAudit.IsInvoiced = lpJTransportation.IsInvoiced;
                        transportationUpdateAudit.IsPaymentVouchered = lpJTransportation.IsPaymentVouchered;
                        transportationUpdateAudit.AuditAction = "UPDATE";
                        transportationUpdateAudit.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJTransportationAudit.Add(transportationUpdateAudit);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        return true;
                    }
                    return false;
                }

                var jobTransportationPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "JobTransportationPrefix");
                var currentJobTransportationNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentJobTransportationNumber");
                string jobTransportationCode = string.Empty;
                int nextNumber = 0;

                if (jobTransportationPrefix != null && currentJobTransportationNumber != null)
                {
                    nextNumber = int.Parse(currentJobTransportationNumber.SystemValue) + 1;
                    jobTransportationCode = $"{jobTransportationPrefix.SystemValue} {nextNumber.ToString("D5")}";

                }
                LpJTransportation transportation = new LpJTransportation();
                transportation.TransportationCode = jobTransportationCode;
                transportation.JobCardId = jobCardTransportationRequest.JobCardId;
                transportation.PaxName = jobCardTransportationRequest.PaxName;
                transportation.CustomerRef = jobCardTransportationRequest.CustomerRef;
                transportation.Remarks = jobCardTransportationRequest.Remarks;
                transportation.Adults = jobCardTransportationRequest.Adults;
                transportation.Children = jobCardTransportationRequest.Children;
                transportation.Infants = jobCardTransportationRequest.Infants;
                transportation.CostBaseAmount = jobCardTransportationRequest.CostBaseAmount;
                transportation.CostTaxAmount = jobCardTransportationRequest.CostTaxAmount;
                transportation.SellBaseAmount = jobCardTransportationRequest.SellBaseAmount;
                transportation.SellTaxAmount = jobCardTransportationRequest.SellTaxAmount;
                transportation.DropoffLocation = jobCardTransportationRequest.DropoffLocation;
                transportation.PickupLocation = jobCardTransportationRequest.PickupLocation;
                transportation.PickupTime = jobCardTransportationRequest.PickupTime;
                transportation.FlightNo = jobCardTransportationRequest.FlightNo;
                transportation.FlightTime = jobCardTransportationRequest.FlightTime;
                transportation.VehicleType = jobCardTransportationRequest.VehicleType;
                transportation.IsVatIncludedCost = jobCardTransportationRequest.IsVatIncludedCost;
                transportation.IsVatIncludedSell = jobCardTransportationRequest.IsVatIncludedSell;
                transportation.Extras = jobCardTransportationRequest.Extras;
                transportation.ExtrasTaxAmount = jobCardTransportationRequest.ExtrasTaxAmount;
                transportation.Parking = jobCardTransportationRequest.Parking;
                transportation.ParkingTaxAmount = jobCardTransportationRequest.ParkingTaxAmount;
                transportation.Water = jobCardTransportationRequest.Water;
                transportation.WaterTaxAmount = jobCardTransportationRequest.WaterTaxAmount;
                transportation.ExtrasSell = jobCardTransportationRequest.ExtrasSell;
                transportation.ExtrasTaxAmountSell = jobCardTransportationRequest.ExtrasTaxAmountSell;
                transportation.ParkingSell = jobCardTransportationRequest.ParkingSell;
                transportation.ParkingTaxAmountSell = jobCardTransportationRequest.ParkingTaxAmountSell;
                transportation.WaterSell = jobCardTransportationRequest.WaterSell;
                transportation.WaterTaxAmountSell = jobCardTransportationRequest.WaterTaxAmountSell;
                transportation.CreatedBy = jobCardTransportationRequest.UserId;
                transportation.CreatedDate = DateTime.UtcNow;
                transportation.UpdatedBy = jobCardTransportationRequest.UserId;
                transportation.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJTransportation.Add(transportation);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJTransportationAudit transportationInsertAudit = new LpJTransportationAudit();
                transportationInsertAudit.Id = transportation.Id;
                transportationInsertAudit.TransportationCode = transportation.TransportationCode;
                transportationInsertAudit.JobCardId = transportation.JobCardId;
                transportationInsertAudit.PaxName = transportation.PaxName;
                transportationInsertAudit.CustomerRef = transportation.CustomerRef;
                transportationInsertAudit.Remarks = transportation.Remarks;
                transportationInsertAudit.Adults = transportation.Adults;
                transportationInsertAudit.Children = transportation.Children;
                transportationInsertAudit.Infants = transportation.Infants;
                transportationInsertAudit.CostBaseAmount = transportation.CostBaseAmount;
                transportationInsertAudit.CostTaxAmount = transportation.CostTaxAmount;
                transportationInsertAudit.SellBaseAmount = transportation.SellBaseAmount;
                transportationInsertAudit.SellTaxAmount = transportation.SellTaxAmount;
                transportationInsertAudit.DropoffLocation = transportation.DropoffLocation;
                transportationInsertAudit.PickupLocation = transportation.PickupLocation;
                transportationInsertAudit.PickupTime = transportation.PickupTime;
                transportationInsertAudit.FlightNo = transportation.FlightNo;
                transportationInsertAudit.FlightTime = transportation.FlightTime;
                transportationInsertAudit.VehicleType = transportation.VehicleType;
                transportationInsertAudit.IsVatIncludedCost = transportation.IsVatIncludedCost;
                transportationInsertAudit.IsVatIncludedSell = transportation.IsVatIncludedSell;
                transportationInsertAudit.Extras = transportation.Extras;
                transportationInsertAudit.ExtrasTaxAmount = transportation.ExtrasTaxAmount;
                transportationInsertAudit.Parking = transportation.Parking;
                transportationInsertAudit.ParkingTaxAmount = transportation.ParkingTaxAmount;
                transportationInsertAudit.Water = transportation.Water;
                transportationInsertAudit.WaterTaxAmount = transportation.WaterTaxAmount;
                transportationInsertAudit.ExtrasSell = transportation.ExtrasSell;
                transportationInsertAudit.ExtrasTaxAmountSell = transportation.ExtrasTaxAmountSell;
                transportationInsertAudit.ParkingSell = transportation.ParkingSell;
                transportationInsertAudit.ParkingTaxAmountSell = transportation.ParkingTaxAmountSell;
                transportationInsertAudit.WaterSell = transportation.WaterSell;
                transportationInsertAudit.WaterTaxAmountSell = transportation.WaterTaxAmountSell;
                transportationInsertAudit.CreatedBy = transportation.CreatedBy;
                transportationInsertAudit.CreatedDate = transportation.CreatedDate;
                transportationInsertAudit.UpdatedBy = transportation.UpdatedBy;
                transportationInsertAudit.UpdatedDate = transportation.UpdatedDate;
                transportationInsertAudit.IsBatched = transportation.IsBatched;
                transportationInsertAudit.IsInvoiced = transportation.IsInvoiced;
                transportationInsertAudit.IsPaymentVouchered = transportation.IsPaymentVouchered;
                transportationInsertAudit.AuditAction = "INSERT";
                transportationInsertAudit.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJTransportationAudit.Add(transportationInsertAudit);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentJobTransportationNumber.SystemValue = nextNumber.ToString();
                currentJobTransportationNumber.UpdatedBy = jobCardTransportationRequest.UserId;
                currentJobTransportationNumber.UpdatedDate = DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentJobTransportationNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<bool> RemoveTransportation(int id)
        {
            var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpJTransportation.Remove(lpJTransportation);
            await dB_LogisticsproContext.SaveChangesAsync();

            LpJTransportationAudit transportationDeleteAudit = new LpJTransportationAudit();
            transportationDeleteAudit.Id = lpJTransportation.Id;
            transportationDeleteAudit.TransportationCode = lpJTransportation.TransportationCode;
            transportationDeleteAudit.JobCardId = lpJTransportation.JobCardId;
            transportationDeleteAudit.PaxName = lpJTransportation.PaxName;
            transportationDeleteAudit.CustomerRef = lpJTransportation.CustomerRef;
            transportationDeleteAudit.Remarks = lpJTransportation.Remarks;
            transportationDeleteAudit.Adults = lpJTransportation.Adults;
            transportationDeleteAudit.Children = lpJTransportation.Children;
            transportationDeleteAudit.Infants = lpJTransportation.Infants;
            transportationDeleteAudit.CostBaseAmount = lpJTransportation.CostBaseAmount;
            transportationDeleteAudit.CostTaxAmount = lpJTransportation.CostTaxAmount;
            transportationDeleteAudit.SellBaseAmount = lpJTransportation.SellBaseAmount;
            transportationDeleteAudit.SellTaxAmount = lpJTransportation.SellTaxAmount;
            transportationDeleteAudit.DropoffLocation = lpJTransportation.DropoffLocation;
            transportationDeleteAudit.PickupLocation = lpJTransportation.PickupLocation;
            transportationDeleteAudit.PickupTime = lpJTransportation.PickupTime;
            transportationDeleteAudit.FlightNo = lpJTransportation.FlightNo;
            transportationDeleteAudit.FlightTime = lpJTransportation.FlightTime;
            transportationDeleteAudit.VehicleType = lpJTransportation.VehicleType;
            transportationDeleteAudit.IsVatIncludedCost = lpJTransportation.IsVatIncludedCost;
            transportationDeleteAudit.IsVatIncludedSell = lpJTransportation.IsVatIncludedSell;
            transportationDeleteAudit.Extras = lpJTransportation.Extras;
            transportationDeleteAudit.ExtrasTaxAmount = lpJTransportation.ExtrasTaxAmount;
            transportationDeleteAudit.Parking = lpJTransportation.Parking;
            transportationDeleteAudit.Water = lpJTransportation.Water;
            transportationDeleteAudit.ExtrasSell = lpJTransportation.ExtrasSell;
            transportationDeleteAudit.ExtrasTaxAmountSell = lpJTransportation.ExtrasTaxAmountSell;
            transportationDeleteAudit.ParkingSell = lpJTransportation.ParkingSell;
            transportationDeleteAudit.WaterSell = lpJTransportation.WaterSell;
            transportationDeleteAudit.CreatedBy = lpJTransportation.CreatedBy;
            transportationDeleteAudit.CreatedDate = lpJTransportation.CreatedDate;
            transportationDeleteAudit.UpdatedBy = lpJTransportation.UpdatedBy;
            transportationDeleteAudit.UpdatedDate = lpJTransportation.UpdatedDate;
            transportationDeleteAudit.IsBatched = lpJTransportation.IsBatched;
            transportationDeleteAudit.IsInvoiced = lpJTransportation.IsInvoiced;
            transportationDeleteAudit.IsPaymentVouchered = lpJTransportation.IsPaymentVouchered;
            transportationDeleteAudit.AuditAction = "DELETE";
            transportationDeleteAudit.AuditDate = DateTime.UtcNow;

            dB_LogisticsproContext.LpJTransportationAudit.Add(transportationDeleteAudit);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SaveJobHotel(JobCardHotelRequest jobCardHotelRequest)
        {
            try
            {
                if (jobCardHotelRequest.Id > 0)
                {
                    var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(i => i.Id == jobCardHotelRequest.Id);
                    if (lpJHotel != null)
                    {
                        lpJHotel.PaxName = jobCardHotelRequest.PaxName;
                        lpJHotel.VendorId = jobCardHotelRequest.VendorId;
                        lpJHotel.Remarks = jobCardHotelRequest.Remarks;
                        lpJHotel.Adults = jobCardHotelRequest.Adults;
                        lpJHotel.Children = jobCardHotelRequest.Children;
                        lpJHotel.Infants = jobCardHotelRequest.Infants;
                        lpJHotel.HotelName = jobCardHotelRequest.HotelName;
                        lpJHotel.HotelAddress1 = jobCardHotelRequest.HotelAddress1;
                        lpJHotel.HotelAddress2 = jobCardHotelRequest.HotelAddress2;
                        lpJHotel.IsVatIncludedCost = jobCardHotelRequest.IsVatIncludedCost;
                        lpJHotel.IsVatIncludedSell = jobCardHotelRequest.IsVatIncludedSell;
                        lpJHotel.CostBaseAmount = jobCardHotelRequest.CostBaseAmount;
                        lpJHotel.CostTaxAmount = jobCardHotelRequest.CostTaxAmount;
                        lpJHotel.SellBaseAmount = jobCardHotelRequest.SellBaseAmount;
                        lpJHotel.SellTaxAmount = jobCardHotelRequest.SellTaxAmount;
                        lpJHotel.CheckIn = jobCardHotelRequest.CheckIn;
                        lpJHotel.CheckOut = jobCardHotelRequest.CheckOut;
                        lpJHotel.HotelConfirmation = jobCardHotelRequest.HotelConfirmation;
                        lpJHotel.RoomType = jobCardHotelRequest.RoomType;
                        lpJHotel.UpdatedBy = jobCardHotelRequest.UserId;
                        lpJHotel.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJHotel).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJHotelAudit hotelUpdateAudit = new LpJHotelAudit();
                        hotelUpdateAudit.Id = lpJHotel.Id;
                        hotelUpdateAudit.HotelCode = lpJHotel.HotelCode;
                        hotelUpdateAudit.JobCardId = lpJHotel.JobCardId;
                        hotelUpdateAudit.PaxName = lpJHotel.PaxName;
                        hotelUpdateAudit.VendorId = lpJHotel.VendorId;
                        hotelUpdateAudit.Remarks = lpJHotel.Remarks;
                        hotelUpdateAudit.Adults = lpJHotel.Adults;
                        hotelUpdateAudit.Children = lpJHotel.Children;
                        hotelUpdateAudit.Infants = lpJHotel.Infants;
                        hotelUpdateAudit.HotelName = lpJHotel.HotelName;
                        hotelUpdateAudit.HotelAddress1 = lpJHotel.HotelAddress1;
                        hotelUpdateAudit.HotelAddress2 = lpJHotel.HotelAddress2;
                        hotelUpdateAudit.IsVatIncludedCost = lpJHotel.IsVatIncludedCost;
                        hotelUpdateAudit.IsVatIncludedSell = lpJHotel.IsVatIncludedSell;
                        hotelUpdateAudit.CostBaseAmount = lpJHotel.CostBaseAmount;
                        hotelUpdateAudit.CostTaxAmount = lpJHotel.CostTaxAmount;
                        hotelUpdateAudit.SellBaseAmount = lpJHotel.SellBaseAmount;
                        hotelUpdateAudit.SellTaxAmount = lpJHotel.SellTaxAmount;
                        hotelUpdateAudit.CheckIn = lpJHotel.CheckIn;
                        hotelUpdateAudit.CheckOut = lpJHotel.CheckOut;
                        hotelUpdateAudit.HotelConfirmation = lpJHotel.HotelConfirmation;
                        hotelUpdateAudit.RoomType = lpJHotel.RoomType;
                        hotelUpdateAudit.CreatedBy = lpJHotel.CreatedBy;
                        hotelUpdateAudit.CreatedDate = lpJHotel.CreatedDate;
                        hotelUpdateAudit.UpdatedBy = lpJHotel.UpdatedBy;
                        hotelUpdateAudit.UpdatedDate = lpJHotel.UpdatedDate;
                        hotelUpdateAudit.IsPaymentVouchered = lpJHotel.IsPaymentVouchered;
                        hotelUpdateAudit.IsInvoiced = lpJHotel.IsInvoiced;
                        hotelUpdateAudit.AuditAction = "UPDATE";
                        hotelUpdateAudit.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJHotelAudit.Add(hotelUpdateAudit);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        return true;
                    }
                    return false;
                }

                var jobHotelPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "JobHotelPrefix");
                var currentJobHotelNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentJobHotelNumber");
                string jobHotelCode = string.Empty;
                int nextNumber = 0;

                if (jobHotelPrefix != null && currentJobHotelNumber != null)
                {
                    nextNumber = int.Parse(currentJobHotelNumber.SystemValue) + 1;
                    jobHotelCode = $"{jobHotelPrefix.SystemValue} {nextNumber.ToString("D4")}";

                }

                LpJHotel hotel = new LpJHotel();
                hotel.HotelCode = jobHotelCode;
                hotel.JobCardId = jobCardHotelRequest.JobCardId;
                hotel.PaxName = jobCardHotelRequest.PaxName;
                hotel.VendorId = jobCardHotelRequest.VendorId;
                hotel.Remarks = jobCardHotelRequest.Remarks;
                hotel.Adults = jobCardHotelRequest.Adults;
                hotel.Children = jobCardHotelRequest.Children;
                hotel.Infants = jobCardHotelRequest.Infants;
                hotel.HotelName = jobCardHotelRequest.HotelName;
                hotel.HotelAddress1 = jobCardHotelRequest.HotelAddress1;
                hotel.HotelAddress2 = jobCardHotelRequest.HotelAddress2;
                hotel.IsVatIncludedCost = jobCardHotelRequest.IsVatIncludedCost;
                hotel.IsVatIncludedSell = jobCardHotelRequest.IsVatIncludedSell;
                hotel.CostBaseAmount = jobCardHotelRequest.CostBaseAmount;
                hotel.CostTaxAmount = jobCardHotelRequest.CostTaxAmount;
                hotel.SellBaseAmount = jobCardHotelRequest.SellBaseAmount;
                hotel.SellTaxAmount = jobCardHotelRequest.SellTaxAmount;
                hotel.CheckIn = jobCardHotelRequest.CheckIn;
                hotel.CheckOut = jobCardHotelRequest.CheckOut;
                hotel.HotelConfirmation = jobCardHotelRequest.HotelConfirmation;
                hotel.RoomType = jobCardHotelRequest.RoomType;
                hotel.CreatedBy = jobCardHotelRequest.UserId;
                hotel.CreatedDate = DateTime.UtcNow;
                hotel.UpdatedBy = jobCardHotelRequest.UserId;
                hotel.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJHotel.Add(hotel);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJHotelAudit hotelInsertAudit = new LpJHotelAudit();
                hotelInsertAudit.Id = hotel.Id;
                hotelInsertAudit.HotelCode = hotel.HotelCode;
                hotelInsertAudit.JobCardId = hotel.JobCardId;
                hotelInsertAudit.PaxName = hotel.PaxName;
                hotelInsertAudit.VendorId = hotel.VendorId;
                hotelInsertAudit.Remarks = hotel.Remarks;
                hotelInsertAudit.Adults = hotel.Adults;
                hotelInsertAudit.Children = hotel.Children;
                hotelInsertAudit.Infants = hotel.Infants;
                hotelInsertAudit.HotelName = hotel.HotelName;
                hotelInsertAudit.HotelAddress1 = hotel.HotelAddress1;
                hotelInsertAudit.HotelAddress2 = hotel.HotelAddress2;
                hotelInsertAudit.IsVatIncludedCost = hotel.IsVatIncludedCost;
                hotelInsertAudit.IsVatIncludedSell = hotel.IsVatIncludedSell;
                hotelInsertAudit.CostBaseAmount = hotel.CostBaseAmount;
                hotelInsertAudit.CostTaxAmount = hotel.CostTaxAmount;
                hotelInsertAudit.SellBaseAmount = hotel.SellBaseAmount;
                hotelInsertAudit.SellTaxAmount = hotel.SellTaxAmount;
                hotelInsertAudit.CheckIn = hotel.CheckIn;
                hotelInsertAudit.CheckOut = hotel.CheckOut;
                hotelInsertAudit.HotelConfirmation = hotel.HotelConfirmation;
                hotelInsertAudit.RoomType = hotel.RoomType;
                hotelInsertAudit.CreatedBy = hotel.CreatedBy;
                hotelInsertAudit.CreatedDate = hotel.CreatedDate;
                hotelInsertAudit.UpdatedBy = hotel.UpdatedBy;
                hotelInsertAudit.UpdatedDate = hotel.UpdatedDate;
                hotelInsertAudit.IsPaymentVouchered = hotel.IsPaymentVouchered;
                hotelInsertAudit.IsInvoiced = hotel.IsInvoiced;
                hotelInsertAudit.AuditAction = "INSERT";
                hotelInsertAudit.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJHotelAudit.Add(hotelInsertAudit);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentJobHotelNumber.SystemValue = nextNumber.ToString();
                currentJobHotelNumber.UpdatedDate = DateTime.UtcNow;
                currentJobHotelNumber.UpdatedBy = jobCardHotelRequest.UserId;
                dB_LogisticsproContext.Entry(currentJobHotelNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<bool> RemoveHotel(int id)
        {
            var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpJHotel.Remove(lpJHotel);
            await dB_LogisticsproContext.SaveChangesAsync();

            LpJHotelAudit hotelDeleteAudit = new LpJHotelAudit();
            hotelDeleteAudit.Id = lpJHotel.Id;
            hotelDeleteAudit.HotelCode = lpJHotel.HotelCode;
            hotelDeleteAudit.JobCardId = lpJHotel.JobCardId;
            hotelDeleteAudit.PaxName = lpJHotel.PaxName;
            hotelDeleteAudit.VendorId = lpJHotel.VendorId;
            hotelDeleteAudit.Remarks = lpJHotel.Remarks;
            hotelDeleteAudit.Adults = lpJHotel.Adults;
            hotelDeleteAudit.Children = lpJHotel.Children;
            hotelDeleteAudit.Infants = lpJHotel.Infants;
            hotelDeleteAudit.HotelName = lpJHotel.HotelName;
            hotelDeleteAudit.IsVatIncludedCost = lpJHotel.IsVatIncludedCost;
            hotelDeleteAudit.IsVatIncludedSell = lpJHotel.IsVatIncludedSell;
            hotelDeleteAudit.CostBaseAmount = lpJHotel.CostBaseAmount;
            hotelDeleteAudit.CostTaxAmount = lpJHotel.CostTaxAmount;
            hotelDeleteAudit.SellBaseAmount = lpJHotel.SellBaseAmount;
            hotelDeleteAudit.SellTaxAmount = lpJHotel.SellTaxAmount;
            hotelDeleteAudit.CheckIn = lpJHotel.CheckIn;
            hotelDeleteAudit.CheckOut = lpJHotel.CheckOut;
            hotelDeleteAudit.HotelConfirmation = lpJHotel.HotelConfirmation;
            hotelDeleteAudit.RoomType = lpJHotel.RoomType;
            hotelDeleteAudit.CreatedBy = lpJHotel.CreatedBy;
            hotelDeleteAudit.CreatedDate = lpJHotel.CreatedDate;
            hotelDeleteAudit.UpdatedBy = lpJHotel.UpdatedBy;
            hotelDeleteAudit.UpdatedDate = lpJHotel.UpdatedDate;
            hotelDeleteAudit.IsPaymentVouchered = lpJHotel.IsPaymentVouchered;
            hotelDeleteAudit.IsInvoiced = lpJHotel.IsInvoiced;
            hotelDeleteAudit.AuditAction = "DELETE";
            hotelDeleteAudit.AuditDate = DateTime.UtcNow;

            dB_LogisticsproContext.LpJHotelAudit.Add(hotelDeleteAudit);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SaveJobVisa(JobCardVisaRequest jobCardVisaRequest)
        {
            try
            {
                if (jobCardVisaRequest.Id > 0)
                {
                    var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(i => i.Id == jobCardVisaRequest.Id);
                    if (lpJVisa != null)
                    {
                        lpJVisa.PaxName = jobCardVisaRequest.PaxName;
                        lpJVisa.PassportNo = jobCardVisaRequest.PassportNo;
                        lpJVisa.VisaTypeId = jobCardVisaRequest.VisaTypeId;
                        lpJVisa.VendorId = jobCardVisaRequest.VendorId;
                        lpJVisa.IsVatIncludedCost = jobCardVisaRequest.IsVatIncludedCost;
                        lpJVisa.IsVatIncludedSell = jobCardVisaRequest.IsVatIncludedSell;
                        lpJVisa.CostBaseAmount = jobCardVisaRequest.CostBaseAmount;
                        lpJVisa.CostTaxAmount = jobCardVisaRequest.CostTaxAmount;
                        lpJVisa.SellBaseAmount = jobCardVisaRequest.SellBaseAmount;
                        lpJVisa.SellTaxAmount = jobCardVisaRequest.SellTaxAmount;
                        lpJVisa.Remarks = jobCardVisaRequest.Remarks;
                        lpJVisa.Nationality = jobCardVisaRequest.Nationality;
                        lpJVisa.UpdatedBy = jobCardVisaRequest.UserId;
                        lpJVisa.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJVisa).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJVisaAudit visaUpdateAudit = new LpJVisaAudit();
                        visaUpdateAudit.Id = lpJVisa.Id;
                        visaUpdateAudit.VisaCode = lpJVisa.VisaCode;
                        visaUpdateAudit.JobCardId = lpJVisa.JobCardId;
                        visaUpdateAudit.VendorId = lpJVisa.VendorId;
                        visaUpdateAudit.PaxName = lpJVisa.PaxName;
                        visaUpdateAudit.PassportNo = lpJVisa.PassportNo;
                        visaUpdateAudit.VisaTypeId = lpJVisa.VisaTypeId;
                        visaUpdateAudit.IsVatIncludedCost = lpJVisa.IsVatIncludedCost;
                        visaUpdateAudit.IsVatIncludedSell = lpJVisa.IsVatIncludedSell;
                        visaUpdateAudit.CostBaseAmount = lpJVisa.CostBaseAmount;
                        visaUpdateAudit.CostTaxAmount = lpJVisa.CostTaxAmount;
                        visaUpdateAudit.SellBaseAmount = lpJVisa.SellBaseAmount;
                        visaUpdateAudit.SellTaxAmount = lpJVisa.SellTaxAmount;
                        visaUpdateAudit.Remarks = lpJVisa.Remarks;
                        visaUpdateAudit.Nationality = lpJVisa.Nationality;
                        visaUpdateAudit.CreatedBy = lpJVisa.CreatedBy;
                        visaUpdateAudit.CreatedDate = lpJVisa.CreatedDate;
                        visaUpdateAudit.UpdatedBy = lpJVisa.UpdatedBy;
                        visaUpdateAudit.UpdatedDate = lpJVisa.UpdatedDate;
                        visaUpdateAudit.IsPaymentVouchered = lpJVisa.IsPaymentVouchered;
                        visaUpdateAudit.IsInvoiced = lpJVisa.IsInvoiced;
                        visaUpdateAudit.AuditAction = "UPDATE";
                        visaUpdateAudit.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJVisaAudit.Add(visaUpdateAudit);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        return true;
                    }
                    return false;
                }

                var jobVisaPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "JobVisaPrefix");
                var currentJobVisaNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentJobVisaNumber");
                string jobVisaCode = string.Empty;
                int nextNumber = 0;

                if (jobVisaPrefix != null && currentJobVisaNumber != null)
                {
                    nextNumber = int.Parse(currentJobVisaNumber.SystemValue) + 1;
                    jobVisaCode = $"{jobVisaPrefix.SystemValue} {nextNumber.ToString("D4")}";

                }

                LpJVisa visa = new LpJVisa();
                visa.VisaCode = jobVisaCode;
                visa.JobCardId = jobCardVisaRequest.JobCardId;
                visa.VendorId = jobCardVisaRequest.VendorId;
                visa.PaxName = jobCardVisaRequest.PaxName;
                visa.PassportNo = jobCardVisaRequest.PassportNo;
                visa.VisaTypeId = jobCardVisaRequest.VisaTypeId;
                visa.IsVatIncludedCost = jobCardVisaRequest.IsVatIncludedCost;
                visa.IsVatIncludedSell = jobCardVisaRequest.IsVatIncludedSell;
                visa.CostBaseAmount = jobCardVisaRequest.CostBaseAmount;
                visa.CostTaxAmount = jobCardVisaRequest.CostTaxAmount;
                visa.SellBaseAmount = jobCardVisaRequest.SellBaseAmount;
                visa.SellTaxAmount = jobCardVisaRequest.SellTaxAmount;
                visa.Remarks = jobCardVisaRequest.Remarks;
                visa.Nationality = jobCardVisaRequest.Nationality;
                visa.CreatedBy = jobCardVisaRequest.UserId;
                visa.CreatedDate = DateTime.UtcNow;
                visa.UpdatedBy = jobCardVisaRequest.UserId;
                visa.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJVisa.Add(visa);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJVisaAudit visaInsertAudit = new LpJVisaAudit();
                visaInsertAudit.Id = visa.Id;
                visaInsertAudit.VisaCode = visa.VisaCode;
                visaInsertAudit.JobCardId = visa.JobCardId;
                visaInsertAudit.VendorId = visa.VendorId;
                visaInsertAudit.PaxName = visa.PaxName;
                visaInsertAudit.PassportNo = visa.PassportNo;
                visaInsertAudit.VisaTypeId = visa.VisaTypeId;
                visaInsertAudit.IsVatIncludedCost = visa.IsVatIncludedCost;
                visaInsertAudit.IsVatIncludedSell = visa.IsVatIncludedSell;
                visaInsertAudit.CostBaseAmount = visa.CostBaseAmount;
                visaInsertAudit.CostTaxAmount = visa.CostTaxAmount;
                visaInsertAudit.SellBaseAmount = visa.SellBaseAmount;
                visaInsertAudit.SellTaxAmount = visa.SellTaxAmount;
                visaInsertAudit.Remarks = visa.Remarks;
                visaInsertAudit.Nationality = visa.Nationality;
                visaInsertAudit.CreatedBy = visa.CreatedBy;
                visaInsertAudit.CreatedDate = visa.CreatedDate;
                visaInsertAudit.UpdatedBy = visa.UpdatedBy;
                visaInsertAudit.UpdatedDate = visa.UpdatedDate;
                visaInsertAudit.IsPaymentVouchered = visa.IsPaymentVouchered;
                visaInsertAudit.IsInvoiced = visa.IsInvoiced;
                visaInsertAudit.AuditAction = "INSERT";
                visaInsertAudit.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJVisaAudit.Add(visaInsertAudit);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentJobVisaNumber.SystemValue = nextNumber.ToString();
                currentJobVisaNumber.UpdatedDate = DateTime.UtcNow;
                currentJobVisaNumber.UpdatedBy = jobCardVisaRequest.UserId;
                dB_LogisticsproContext.Entry(currentJobVisaNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<bool> RemoveVisa(int id)
        {
            var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpJVisa.Remove(lpJVisa);
            await dB_LogisticsproContext.SaveChangesAsync();

            LpJVisaAudit visaUpdateAudit = new LpJVisaAudit();
            visaUpdateAudit.Id = lpJVisa.Id;
            visaUpdateAudit.VisaCode = lpJVisa.VisaCode;
            visaUpdateAudit.JobCardId = lpJVisa.JobCardId;
            visaUpdateAudit.VendorId = lpJVisa.VendorId;
            visaUpdateAudit.PaxName = lpJVisa.PaxName;
            visaUpdateAudit.PassportNo = lpJVisa.PassportNo;
            visaUpdateAudit.VisaTypeId = lpJVisa.VisaTypeId;
            visaUpdateAudit.IsVatIncludedCost = lpJVisa.IsVatIncludedCost;
            visaUpdateAudit.IsVatIncludedSell = lpJVisa.IsVatIncludedSell;
            visaUpdateAudit.CostBaseAmount = lpJVisa.CostBaseAmount;
            visaUpdateAudit.CostTaxAmount = lpJVisa.CostTaxAmount;
            visaUpdateAudit.SellBaseAmount = lpJVisa.SellBaseAmount;
            visaUpdateAudit.SellTaxAmount = lpJVisa.SellTaxAmount;
            visaUpdateAudit.Remarks = lpJVisa.Remarks;
            visaUpdateAudit.Nationality = lpJVisa.Nationality;
            visaUpdateAudit.CreatedBy = lpJVisa.CreatedBy;
            visaUpdateAudit.CreatedDate = lpJVisa.CreatedDate;
            visaUpdateAudit.UpdatedBy = lpJVisa.UpdatedBy;
            visaUpdateAudit.UpdatedDate = lpJVisa.UpdatedDate;
            visaUpdateAudit.IsPaymentVouchered = lpJVisa.IsPaymentVouchered;
            visaUpdateAudit.IsInvoiced = lpJVisa.IsInvoiced;
            visaUpdateAudit.AuditAction = "DELETE";
            visaUpdateAudit.AuditDate = DateTime.UtcNow;

            dB_LogisticsproContext.LpJVisaAudit.Add(visaUpdateAudit);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveJobMiscellaneous(JobCardMiscellaneousRequest jobCardMiscellaneousRequest)
        {
            try
            {
                if (jobCardMiscellaneousRequest.Id > 0)
                {
                    var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(i => i.Id == jobCardMiscellaneousRequest.Id);
                    if (lpJMiscellaneous != null)
                    {
                        lpJMiscellaneous.VendorId = jobCardMiscellaneousRequest.VendorId;
                        lpJMiscellaneous.PaxName = jobCardMiscellaneousRequest.PaxName;
                        lpJMiscellaneous.PaxNumber = jobCardMiscellaneousRequest.PaxNumber;
                        lpJMiscellaneous.Description = jobCardMiscellaneousRequest.Description;
                        lpJMiscellaneous.MisDate = jobCardMiscellaneousRequest.MisDate;
                        lpJMiscellaneous.Remarks = jobCardMiscellaneousRequest.Remarks;
                        lpJMiscellaneous.IsVatIncludedCost = jobCardMiscellaneousRequest.IsVatIncludedCost;
                        lpJMiscellaneous.IsVatIncludedSell = jobCardMiscellaneousRequest.IsVatIncludedSell;
                        lpJMiscellaneous.CostBaseAmount = jobCardMiscellaneousRequest.CostBaseAmount;
                        lpJMiscellaneous.CostTaxAmount = jobCardMiscellaneousRequest.CostTaxAmount;
                        lpJMiscellaneous.SellBaseAmount = jobCardMiscellaneousRequest.SellBaseAmount;
                        lpJMiscellaneous.SellTaxAmount = jobCardMiscellaneousRequest.SellTaxAmount;
                        lpJMiscellaneous.IsFinance = jobCardMiscellaneousRequest.IsFinance;

                        lpJMiscellaneous.UpdatedBy = jobCardMiscellaneousRequest.UserId;
                        lpJMiscellaneous.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJMiscellaneous).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJMiscellaneousAudit miscellaneousUpdateAudit = new LpJMiscellaneousAudit();
                        miscellaneousUpdateAudit.Id = lpJMiscellaneous.Id;
                        miscellaneousUpdateAudit.MiscellaneousCode = lpJMiscellaneous.MiscellaneousCode;
                        miscellaneousUpdateAudit.VendorId = lpJMiscellaneous.VendorId;
                        miscellaneousUpdateAudit.JobCardId = lpJMiscellaneous.JobCardId;
                        miscellaneousUpdateAudit.PaxName = lpJMiscellaneous.PaxName;
                        miscellaneousUpdateAudit.PaxNumber = lpJMiscellaneous.PaxNumber;
                        miscellaneousUpdateAudit.Description = lpJMiscellaneous.Description;
                        miscellaneousUpdateAudit.Remarks = lpJMiscellaneous.Remarks;
                        miscellaneousUpdateAudit.MisDate = lpJMiscellaneous.MisDate;
                        miscellaneousUpdateAudit.IsVatIncludedCost = lpJMiscellaneous.IsVatIncludedCost;
                        miscellaneousUpdateAudit.IsVatIncludedSell = lpJMiscellaneous.IsVatIncludedSell;
                        miscellaneousUpdateAudit.CostBaseAmount = lpJMiscellaneous.CostBaseAmount;
                        miscellaneousUpdateAudit.CostTaxAmount = lpJMiscellaneous.CostTaxAmount;
                        miscellaneousUpdateAudit.SellBaseAmount = lpJMiscellaneous.SellBaseAmount;
                        miscellaneousUpdateAudit.SellTaxAmount = lpJMiscellaneous.SellTaxAmount;
                        miscellaneousUpdateAudit.IsFinance = lpJMiscellaneous.IsFinance;
                        miscellaneousUpdateAudit.CreatedBy = lpJMiscellaneous.CreatedBy;
                        miscellaneousUpdateAudit.CreatedDate = lpJMiscellaneous.CreatedDate;
                        miscellaneousUpdateAudit.UpdatedBy = lpJMiscellaneous.UpdatedBy;
                        miscellaneousUpdateAudit.UpdatedDate = lpJMiscellaneous.UpdatedDate;
                        miscellaneousUpdateAudit.IsPaymentVouchered = lpJMiscellaneous.IsPaymentVouchered;
                        miscellaneousUpdateAudit.IsInvoiced = lpJMiscellaneous.IsInvoiced;
                        miscellaneousUpdateAudit.IsFinance = lpJMiscellaneous.IsFinance;
                        miscellaneousUpdateAudit.AuditAction = "UPDATE";
                        miscellaneousUpdateAudit.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJMiscellaneousAudit.Add(miscellaneousUpdateAudit);
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                var jobMiscellaneousPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "JobMiscellaneousPrefix");
                var currentJobMiscellaneousNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentJobMiscellaneousNumber");
                string jobMiscellaneousCode = string.Empty;
                int nextNumber = 0;

                if (jobMiscellaneousPrefix != null && currentJobMiscellaneousNumber != null)
                {
                    nextNumber = int.Parse(currentJobMiscellaneousNumber.SystemValue) + 1;
                    jobMiscellaneousCode = $"{jobMiscellaneousPrefix.SystemValue} {nextNumber.ToString("D4")}";

                }

                LpJMiscellaneous miscellaneous = new LpJMiscellaneous();
                miscellaneous.MiscellaneousCode = jobMiscellaneousCode;
                miscellaneous.VendorId = jobCardMiscellaneousRequest.VendorId;
                miscellaneous.JobCardId = jobCardMiscellaneousRequest.JobCardId;
                miscellaneous.PaxName = jobCardMiscellaneousRequest.PaxName;
                miscellaneous.PaxNumber = jobCardMiscellaneousRequest.PaxNumber;
                miscellaneous.Description = jobCardMiscellaneousRequest.Description;
                miscellaneous.Remarks = jobCardMiscellaneousRequest.Remarks;
                miscellaneous.MisDate = jobCardMiscellaneousRequest.MisDate;
                miscellaneous.IsVatIncludedCost = jobCardMiscellaneousRequest.IsVatIncludedCost;
                miscellaneous.IsVatIncludedSell = jobCardMiscellaneousRequest.IsVatIncludedSell;
                miscellaneous.CostBaseAmount = jobCardMiscellaneousRequest.CostBaseAmount;
                miscellaneous.CostTaxAmount = jobCardMiscellaneousRequest.CostTaxAmount;
                miscellaneous.SellBaseAmount = jobCardMiscellaneousRequest.SellBaseAmount;
                miscellaneous.SellTaxAmount = jobCardMiscellaneousRequest.SellTaxAmount;
                miscellaneous.IsFinance = jobCardMiscellaneousRequest.IsFinance;
                miscellaneous.CreatedBy = jobCardMiscellaneousRequest.UserId;
                miscellaneous.CreatedDate = DateTime.UtcNow;
                miscellaneous.UpdatedBy = jobCardMiscellaneousRequest.UserId;
                miscellaneous.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJMiscellaneous.Add(miscellaneous);
                await dB_LogisticsproContext.SaveChangesAsync();

                LpJMiscellaneousAudit miscellaneousInsertAudit = new LpJMiscellaneousAudit();
                miscellaneousInsertAudit.Id = miscellaneous.Id;
                miscellaneousInsertAudit.MiscellaneousCode = miscellaneous.MiscellaneousCode;
                miscellaneousInsertAudit.VendorId = miscellaneous.VendorId;
                miscellaneousInsertAudit.JobCardId = miscellaneous.JobCardId;
                miscellaneousInsertAudit.PaxName = miscellaneous.PaxName;
                miscellaneousInsertAudit.PaxNumber = miscellaneous.PaxNumber;
                miscellaneousInsertAudit.Description = miscellaneous.Description;
                miscellaneousInsertAudit.Remarks = miscellaneous.Remarks;
                miscellaneousInsertAudit.MisDate = miscellaneous.MisDate;
                miscellaneousInsertAudit.IsVatIncludedCost = miscellaneous.IsVatIncludedCost;
                miscellaneousInsertAudit.IsVatIncludedSell = miscellaneous.IsVatIncludedSell;
                miscellaneousInsertAudit.CostBaseAmount = miscellaneous.CostBaseAmount;
                miscellaneousInsertAudit.CostTaxAmount = miscellaneous.CostTaxAmount;
                miscellaneousInsertAudit.SellBaseAmount = miscellaneous.SellBaseAmount;
                miscellaneousInsertAudit.SellTaxAmount = miscellaneous.SellTaxAmount;
                miscellaneousInsertAudit.IsFinance = miscellaneous.IsFinance;
                miscellaneousInsertAudit.CreatedBy = miscellaneous.CreatedBy;
                miscellaneousInsertAudit.CreatedDate = miscellaneous.CreatedDate;
                miscellaneousInsertAudit.UpdatedBy = miscellaneous.UpdatedBy;
                miscellaneousInsertAudit.UpdatedDate = miscellaneous.UpdatedDate;
                miscellaneousInsertAudit.IsPaymentVouchered = miscellaneous.IsPaymentVouchered;
                miscellaneousInsertAudit.IsInvoiced = miscellaneous.IsInvoiced;
                miscellaneousInsertAudit.IsFinance = miscellaneous.IsFinance;
                miscellaneousInsertAudit.AuditAction = "INSERT";
                miscellaneousInsertAudit.AuditDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpJMiscellaneousAudit.Add(miscellaneousInsertAudit);
                await dB_LogisticsproContext.SaveChangesAsync();


                currentJobMiscellaneousNumber.SystemValue = nextNumber.ToString();
                currentJobMiscellaneousNumber.UpdatedDate= DateTime.UtcNow;
                currentJobMiscellaneousNumber.UpdatedBy = jobCardMiscellaneousRequest.UserId;
                dB_LogisticsproContext.Entry(currentJobMiscellaneousNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<bool> RemoveMiscellaneous(int id)
        {
            var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpJMiscellaneous.Remove(lpJMiscellaneous);
            await dB_LogisticsproContext.SaveChangesAsync();

            LpJMiscellaneousAudit miscellaneousUpdateAudit = new LpJMiscellaneousAudit();
            miscellaneousUpdateAudit.Id = lpJMiscellaneous.Id;
            miscellaneousUpdateAudit.MiscellaneousCode = lpJMiscellaneous.MiscellaneousCode;
            miscellaneousUpdateAudit.VendorId = lpJMiscellaneous.VendorId;
            miscellaneousUpdateAudit.JobCardId = lpJMiscellaneous.JobCardId;
            miscellaneousUpdateAudit.PaxName = lpJMiscellaneous.PaxName;
            miscellaneousUpdateAudit.PaxNumber = lpJMiscellaneous.PaxNumber;
            miscellaneousUpdateAudit.Description = lpJMiscellaneous.Description;
            miscellaneousUpdateAudit.Remarks = lpJMiscellaneous.Remarks;
            miscellaneousUpdateAudit.MisDate = lpJMiscellaneous.MisDate;
            miscellaneousUpdateAudit.IsVatIncludedCost = lpJMiscellaneous.IsVatIncludedCost;
            miscellaneousUpdateAudit.IsVatIncludedSell = lpJMiscellaneous.IsVatIncludedSell;
            miscellaneousUpdateAudit.CostBaseAmount = lpJMiscellaneous.CostBaseAmount;
            miscellaneousUpdateAudit.CostTaxAmount = lpJMiscellaneous.CostTaxAmount;
            miscellaneousUpdateAudit.SellBaseAmount = lpJMiscellaneous.SellBaseAmount;
            miscellaneousUpdateAudit.SellTaxAmount = lpJMiscellaneous.SellTaxAmount;
            miscellaneousUpdateAudit.IsFinance = lpJMiscellaneous.IsFinance;
            miscellaneousUpdateAudit.CreatedBy = lpJMiscellaneous.CreatedBy;
            miscellaneousUpdateAudit.CreatedDate = lpJMiscellaneous.CreatedDate;
            miscellaneousUpdateAudit.UpdatedBy = lpJMiscellaneous.UpdatedBy;
            miscellaneousUpdateAudit.UpdatedDate = lpJMiscellaneous.UpdatedDate;
            miscellaneousUpdateAudit.IsPaymentVouchered = lpJMiscellaneous.IsPaymentVouchered;
            miscellaneousUpdateAudit.IsInvoiced = lpJMiscellaneous.IsInvoiced;
            miscellaneousUpdateAudit.IsFinance = lpJMiscellaneous.IsFinance;
            miscellaneousUpdateAudit.AuditAction = "DELETE";
            miscellaneousUpdateAudit.AuditDate = DateTime.UtcNow;

            dB_LogisticsproContext.LpJMiscellaneousAudit.Add(miscellaneousUpdateAudit);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }

        public async Task<LineGenerateItem> GetFullReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var returnItems = new LineGenerateItem();

                var reportItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                         join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                         join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                         where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                                  (bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                                  (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                                  (clientRef == null || lpt.CustomerRef == clientRef) &&
                                                  (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date >= dateFrom.Value.Date)) &&
                                                  (dateTo == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date <= dateTo.Value.Date)) &&
                                                  lpt.IsBatched == false && (batchNo == null && (vendorId== null || vendorId.Count == 0))
                                         select new JobCardTransportation
                                         {
                                             Id = lpt.Id,
                                             BookingRef = lpt.TransportationCode,
                                             CustomerRef = lpt.CustomerRef,
                                             Remarks = lpt.Remarks,
                                             DropoffLocation = lpt.DropoffLocation,
                                             CostBaseAmount = lpt.CostBaseAmount,
                                             CostTaxAmount = lpt.CostTaxAmount,
                                             IsVatIncludedSell = lpt.IsVatIncludedSell,
                                             IsVatIncludedCost = lpt.IsVatIncludedCost,
                                             SellBaseAmount = lpt.SellBaseAmount,
                                             SellTaxAmount = lpt.SellTaxAmount,
                                             JobCardNo = jc.JobCardCode,
                                             PickupLocation = lpt.PickupLocation,
                                             PickupTime = lpt.PickupTime,
                                             Adults = lpt.Adults,
                                             Children = lpt.Children,
                                             Infants = lpt.Infants,
                                             VehicleType = lpt.VehicleType,
                                             FlightNo = lpt.FlightNo,
                                             FlightTime = lpt.FlightTime,
                                             PaxName = lpt.PaxName,
                                             CustomerName = c.CustomerName,
                                             Parking = lpt.Parking,
                                             ParkingTaxAmount= lpt.ParkingTaxAmount,
                                             Extras = lpt.Extras,
                                             ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                             Water = lpt.Water,
                                             WaterTaxAmount = lpt.WaterTaxAmount,
                                             ParkingSell = lpt.ParkingSell,
                                             ParkingTaxAmountSell = lpt.ParkingTaxAmountSell,
                                             ExtrasSell = lpt.ExtrasSell,
                                             ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                             WaterSell = lpt.WaterSell,
                                             WaterTaxAmountSell = lpt.WaterTaxAmountSell
                                         }).OrderByDescending(i => i.Id).ToListAsync();


                var reportBatchItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                              join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                              join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                              join bl in dB_LogisticsproContext.LpLBatchLineItem on lpt.Id equals bl.ContextId
                                              join b in dB_LogisticsproContext.LpLBatch on bl.BatchId equals b.Id
                                              join v in dB_LogisticsproContext.LpMVender on b.VenderId equals v.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                                       (bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                                       (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                                       (clientRef == null || lpt.CustomerRef == clientRef) &&
                                                       (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date >= dateFrom.Value.Date)) &&
                                                   (dateTo == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date <= dateTo.Value.Date)) &&
                                                       (batchNo == null || b.BatchCode.Contains(batchNo)) &&
                                                          (vendorId == null || vendorId.Count == 0 || (b.VenderId!=null && vendorId.Any(x => b.VenderId.Value == x))) &&
                                                          lpt.IsBatched == true
                                              select new JobCardTransportation
                                              {
                                                  Id = lpt.Id,
                                                  BookingRef = lpt.TransportationCode,
                                                  CustomerRef = lpt.CustomerRef,
                                                  Remarks = lpt.Remarks,
                                                  DropoffLocation = lpt.DropoffLocation,
                                                  CostBaseAmount = lpt.CostBaseAmount,
                                                  CostTaxAmount = lpt.CostTaxAmount,
                                                  IsVatIncludedSell = lpt.IsVatIncludedSell,
                                                  IsVatIncludedCost = lpt.IsVatIncludedCost,
                                                  SellBaseAmount = lpt.SellBaseAmount,
                                                  SellTaxAmount = lpt.SellTaxAmount,
                                                  JobCardNo = jc.JobCardCode,
                                                  PickupLocation = lpt.PickupLocation,
                                                  PickupTime = lpt.PickupTime,
                                                  Adults = lpt.Adults,
                                                  Children = lpt.Children,
                                                  Infants = lpt.Infants,
                                                  VehicleType = lpt.VehicleType,
                                                  FlightNo = lpt.FlightNo,
                                                  FlightTime = lpt.FlightTime,
                                                  PaxName = lpt.PaxName,
                                                  CustomerName = c.CustomerName,
                                                  SuplierName = v.VenderName,
                                                  BatchNo = b.BatchCode,
                                                  Parking = lpt.Parking,
                                                  ParkingTaxAmount = lpt.ParkingTaxAmount,
                                             Extras = lpt.Extras,
                                             ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                             Water = lpt.Water,
                                             WaterTaxAmount = lpt.WaterTaxAmount,
                                             ParkingSell = lpt.ParkingSell,
                                             ParkingTaxAmountSell = lpt.ParkingTaxAmountSell,
                                             ExtrasSell = lpt.ExtrasSell,
                                             ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                             WaterSell = lpt.WaterSell,
                                             WaterTaxAmountSell = lpt.WaterTaxAmountSell
                                              }).OrderByDescending(i => i.Id).ToListAsync();

                reportItems.AddRange(reportBatchItems);

                returnItems.Transportations.AddRange(reportItems.Distinct().OrderBy(i => i.PickupTime).ToList());


                var batchHotelItems = await (from j in dB_LogisticsproContext.LpJobCard
                                             join ljh in dB_LogisticsproContext.LpJHotel on j.Id equals ljh.JobCardId
                                             join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                             join v in dB_LogisticsproContext.LpMVender on ljh.VendorId equals v.Id
                                             where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                             (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljh.VendorId.Value == x)) &&
                                             (dateFrom == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date >= dateFrom.Value.Date)) &&
                                             (dateTo == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date <= dateTo.Value.Date))
                                             select new JobCardHotel
                                             {
                                                 Id = ljh.Id,
                                                 JobCardNo = j.JobCardCode,
                                                 HotelCode = ljh.HotelCode,
                                                 PaxName = ljh.PaxName,
                                                 CheckIn = ljh.CheckIn,
                                                 CheckOut = ljh.CheckOut,
                                                 Adults = ljh.Adults,
                                                 Children = ljh.Children,
                                                 Infants = ljh.Infants,
                                                 HotelName = ljh.HotelName,
                                                 IsVatIncludedCost = ljh.IsVatIncludedCost,
                                                 IsVatIncludedSell = ljh.IsVatIncludedSell,
                                                 CostBaseAmount = ljh.CostBaseAmount,
                                                 CostTaxAmount = ljh.CostTaxAmount,
                                                 SellBaseAmount = ljh.SellBaseAmount,
                                                 SellTaxAmount = ljh.SellTaxAmount,
                                                 Remarks = ljh.Remarks,
                                                 RoomType = ljh.RoomType,
                                                 HotelConfirmation = ljh.HotelConfirmation,
                                                 VenderName = v.VenderName
                                             }).ToListAsync();

                returnItems.Hotels.AddRange(batchHotelItems.Distinct().OrderBy(i => i.CheckIn).ToList());

                var batchVisaItems = await (from j in dB_LogisticsproContext.LpJobCard
                                            join ljv in dB_LogisticsproContext.LpJVisa on j.Id equals ljv.JobCardId
                                            join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                            join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                            join v in dB_LogisticsproContext.LpMVender on ljv.VendorId equals v.Id
                                            where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                             (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljv.VendorId.Value == x)) &&
                                             (dateFrom == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date >= dateFrom.Value.Date)) &&
                                             (dateTo == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date <= dateTo.Value.Date))
                                            select new JobCardVisa
                                            {
                                                Id = ljv.Id,
                                                JobCardNo = j.JobCardCode,
                                                VisaCode = ljv.VisaCode,
                                                VisaTypeId = ljv.VisaTypeId,
                                                PaxName = ljv.PaxName,
                                                PassportNo = ljv.PassportNo,
                                                VisaTypeName = lpvt.VisaTypeName,
                                                IsVatIncludedCost = ljv.IsVatIncludedCost,
                                                IsVatIncludedSell = ljv.IsVatIncludedSell,
                                                CostBaseAmount = ljv.CostBaseAmount,
                                                CostTaxAmount = ljv.CostTaxAmount,
                                                SellBaseAmount = ljv.SellBaseAmount,
                                                SellTaxAmount = ljv.SellTaxAmount,
                                                JobCardId = ljv.JobCardId,
                                                Nationality = ljv.Nationality,
                                                Remarks = ljv.Remarks,
                                                VendorName = v.VenderName
                                            }).ToListAsync();

                returnItems.Visa.AddRange(batchVisaItems.Distinct().OrderBy(i => i.CreatedDate).ToList());


                var batchMiscellaneousItems = await (from j in dB_LogisticsproContext.LpJobCard
                                                     join ljm in dB_LogisticsproContext.LpJMiscellaneous on j.Id equals ljm.JobCardId
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     join v in dB_LogisticsproContext.LpMVender on ljm.VendorId equals v.Id
                                                     where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                                     (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljm.VendorId.Value == x)) &&
                                                     (dateFrom == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date >= dateFrom.Value.Date)) &&
                                                     (dateTo == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date <= dateTo.Value.Date))
                                                     select new JobCardMiscellaneous
                                                     {
                                                         Id = ljm.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         MiscellaneousCode = ljm.MiscellaneousCode,
                                                         PaxName = ljm.PaxName,
                                                         PaxNumber = ljm.PaxNumber,
                                                         Description = ljm.Description,
                                                         MisDate = ljm.MisDate,
                                                         IsVatIncludedCost = ljm.IsVatIncludedCost,
                                                         IsVatIncludedSell = ljm.IsVatIncludedSell,
                                                         CostBaseAmount = ljm.CostBaseAmount,
                                                         CostTaxAmount = ljm.CostTaxAmount,
                                                         SellBaseAmount = ljm.SellBaseAmount,
                                                         SellTaxAmount = ljm.SellTaxAmount,
                                                         JobCardId = ljm.JobCardId,
                                                         Remarks = ljm.Remarks,
                                                         VendorName = v.VenderName
                                                     }).ToListAsync();

                returnItems.Miscellaneous.AddRange(batchMiscellaneousItems.Distinct().OrderBy(i => i.MisDate).ToList());

                return returnItems;
            }
            catch (Exception e)
            {

                throw;
            }
            

        }

        public async Task<HistoryGenerateItem> GetHistoryReportItems(string? jobCardCode, List<int>? userId, DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var returnItems = new HistoryGenerateItem();

                var jobCards = await (from j in dB_LogisticsproContext.LpJobCardAudit
                                      join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                      join s in dB_LogisticsproContext.LpMStatus on j.StatusId equals s.Id
                                      where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                       (userId == null || userId.Count == 0 || userId.Any(x => j.UpdatedBy.Value == x)) &&
                                        (dateFrom == null || (j.AuditDate.HasValue && j.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                        (dateTo == null || (j.AuditDate.HasValue && j.AuditDate.Value.Date <= dateTo.Value.Date)) &&
                                        j.AuditAction!="DELETE"
                                      select new JobCardHistory
                                      {
                                          Id=j.Id==null?0:j.Id.Value,
                                          CreatedBy=j.CreatedBy,
                                          CreatedDate=j.CreatedDate,
                                          CustomerId=j.CustomerId,
                                          CustomerName=c.CustomerName,
                                          CustomerRef=j.CustomerRef,
                                          EffectiveDate=j.EffectiveDate,
                                          JobCardCode=j.JobCardCode,
                                          JobCardDescription=j.JobDescription,
                                          Remarks=j.Remarks,
                                          Status=s.StatusName,
                                          UpdatedBy=j.UpdatedBy,
                                          AuditAction=j.AuditAction,
                                          AuditDate=j.AuditDate,
                                          UpdatedDate=j.AuditDate
                                      }).ToListAsync();

                foreach (var jobCard in jobCards)
                {
                    jobCard.CreatedByName = await GetUser(jobCard.CreatedBy);
                    jobCard.UpdatedByName = await GetUser(jobCard.UpdatedBy);
                }

                returnItems.JobCards.AddRange(jobCards);

                var transportations = await (from lpt in dB_LogisticsproContext.LpJTransportationAudit
                                             join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                             join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                             where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                            (dateFrom == null || (lpt.AuditDate.HasValue && lpt.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                            (dateTo == null || (lpt.AuditDate.HasValue && lpt.AuditDate.Value.Date <= dateTo.Value.Date)) &&
                                            ((userId == null || userId.Any(x => lpt.CreatedBy.Value == x)) || (userId == null || userId.Any(x => lpt.UpdatedBy.Value == x)))
                                             select new JobCardTransportationHistory
                                             {
                                                 Id = lpt.Id == null ? 0 : lpt.Id.Value,
                                                 BookingRef = lpt.TransportationCode,
                                                 CustomerRef = lpt.CustomerRef,
                                                 Remarks = lpt.Remarks,
                                                 DropoffLocation = lpt.DropoffLocation,
                                                 CostBaseAmount = lpt.CostBaseAmount,
                                                 CostTaxAmount = lpt.CostTaxAmount,
                                                 IsVatIncludedSell = lpt.IsVatIncludedSell,
                                                 IsVatIncludedCost = lpt.IsVatIncludedCost,
                                                 SellBaseAmount = lpt.SellBaseAmount,
                                                 SellTaxAmount = lpt.SellTaxAmount,
                                                 JobCardNo = jc.JobCardCode,
                                                 PickupLocation = lpt.PickupLocation,
                                                 PickupTime = lpt.PickupTime,
                                                 Adults = lpt.Adults,
                                                 Children = lpt.Children,
                                                 Infants = lpt.Infants,
                                                 VehicleType = lpt.VehicleType,
                                                 FlightNo = lpt.FlightNo,
                                                 FlightTime = lpt.FlightTime,
                                                 PaxName = lpt.PaxName,
                                                 CustomerName = c.CustomerName,
                                                 Parking = lpt.Parking,
                                                 Extras = lpt.Extras,
                                                 ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                                 Water = lpt.Water,
                                                 ParkingSell = lpt.ParkingSell,
                                                 ExtrasSell = lpt.ExtrasSell,
                                                 ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                                 WaterSell = lpt.WaterSell,
                                                 IsBatched = lpt.IsBatched,
                                                 IsPaymentVouchered = lpt.IsPaymentVouchered,
                                                 IsInvoiced = lpt.IsInvoiced,
                                                 AuditAction = lpt.AuditAction,
                                                 AuditDate = lpt.AuditDate,
                                                 CreatedBy = lpt.CreatedBy,
                                                 CreatedDate = lpt.CreatedDate,
                                                 UpdatedBy = lpt.UpdatedBy,
                                                 UpdatedDate = lpt.UpdatedDate,
                                             }).OrderByDescending(i => i.Id).ToListAsync();

                var deleteTransportations = await (from lpt in dB_LogisticsproContext.LpJTransportationAudit
                                             join jc in dB_LogisticsproContext.LpJobCardAudit on lpt.JobCardId equals jc.Id
                                             join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                             where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                            (dateFrom == null || (lpt.AuditDate.HasValue && lpt.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                            (dateTo == null || (lpt.AuditDate.HasValue && lpt.AuditDate.Value.Date <= dateTo.Value.Date)) &&
                                                ((userId == null || userId.Any(x => lpt.CreatedBy.Value == x)) || (userId == null || userId.Any(x => lpt.UpdatedBy.Value == x))) &&
                                            jc.AuditAction=="DELETE"
                                             select new JobCardTransportationHistory
                                             {
                                                 Id = lpt.Id == null ? 0 : lpt.Id.Value,
                                                 BookingRef = lpt.TransportationCode,
                                                 CustomerRef = lpt.CustomerRef,
                                                 Remarks = lpt.Remarks,
                                                 DropoffLocation = lpt.DropoffLocation,
                                                 CostBaseAmount = lpt.CostBaseAmount,
                                                 CostTaxAmount = lpt.CostTaxAmount,
                                                 IsVatIncludedSell = lpt.IsVatIncludedSell,
                                                 IsVatIncludedCost = lpt.IsVatIncludedCost,
                                                 SellBaseAmount = lpt.SellBaseAmount,
                                                 SellTaxAmount = lpt.SellTaxAmount,
                                                 JobCardNo = jc.JobCardCode,
                                                 PickupLocation = lpt.PickupLocation,
                                                 PickupTime = lpt.PickupTime,
                                                 Adults = lpt.Adults,
                                                 Children = lpt.Children,
                                                 Infants = lpt.Infants,
                                                 VehicleType = lpt.VehicleType,
                                                 FlightNo = lpt.FlightNo,
                                                 FlightTime = lpt.FlightTime,
                                                 PaxName = lpt.PaxName,
                                                 CustomerName = c.CustomerName,
                                                 Parking = lpt.Parking,
                                                 Extras = lpt.Extras,
                                                 ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                                 Water = lpt.Water,
                                                 ParkingSell = lpt.ParkingSell,
                                                 ExtrasSell = lpt.ExtrasSell,
                                                 ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                                 WaterSell = lpt.WaterSell,
                                                 IsBatched = lpt.IsBatched,
                                                 IsPaymentVouchered = lpt.IsPaymentVouchered,
                                                 IsInvoiced = lpt.IsInvoiced,
                                                 AuditAction = lpt.AuditAction,
                                                 AuditDate = lpt.AuditDate,
                                                 CreatedBy = lpt.CreatedBy,
                                                 CreatedDate = lpt.CreatedDate,
                                                 UpdatedBy = lpt.UpdatedBy,
                                                 UpdatedDate = lpt.UpdatedDate,
                                             }).OrderByDescending(i => i.Id).ToListAsync();

                transportations.AddRange(deleteTransportations);

                foreach (var transportation in transportations)
                {
                    transportation.CreatedByName = await GetUser(transportation.CreatedBy);
                    transportation.UpdatedByName = await GetUser(transportation.UpdatedBy);
                }

                returnItems.Transportations.AddRange(transportations.Distinct().ToList());


                var hotels = await (from ljh in dB_LogisticsproContext.LpJHotelAudit
                                    join j in dB_LogisticsproContext.LpJobCard on ljh.JobCardId equals j.Id
                                    join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                    join v in dB_LogisticsproContext.LpMVender on ljh.VendorId equals v.Id
                                    where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                       ((userId == null || userId.Any(x => ljh.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljh.UpdatedBy.Value == x))) &&
                                    (dateFrom == null || (ljh.AuditDate.HasValue && ljh.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                    (dateTo == null || (ljh.AuditDate.HasValue && ljh.AuditDate.Value.Date <= dateTo.Value.Date))
                                    select new JobCardHotelHistory
                                    {
                                        Id = ljh.Id == null ? 0 : ljh.Id.Value,
                                        JobCardNo = j.JobCardCode,
                                        HotelCode = ljh.HotelCode,
                                        PaxName = ljh.PaxName,
                                        CheckIn = ljh.CheckIn,
                                        CheckOut = ljh.CheckOut,
                                        Adults = ljh.Adults,
                                        Children = ljh.Children,
                                        Infants = ljh.Infants,
                                        HotelName = ljh.HotelName,
                                        IsVatIncludedCost = ljh.IsVatIncludedCost,
                                        IsVatIncludedSell = ljh.IsVatIncludedSell,
                                        CostBaseAmount = ljh.CostBaseAmount,
                                        CostTaxAmount = ljh.CostTaxAmount,
                                        SellBaseAmount = ljh.SellBaseAmount,
                                        SellTaxAmount = ljh.SellTaxAmount,
                                        Remarks = ljh.Remarks,
                                        RoomType = ljh.RoomType,
                                        HotelConfirmation = ljh.HotelConfirmation,
                                        VenderName = v.VenderName,
                                        IsPaymentVouchered = ljh.IsPaymentVouchered,
                                        IsInvoiced = ljh.IsInvoiced,
                                        AuditAction = ljh.AuditAction,
                                        AuditDate = ljh.AuditDate,
                                        CreatedBy = ljh.CreatedBy,
                                        CreatedDate = ljh.CreatedDate,
                                        UpdatedBy = ljh.UpdatedBy,
                                        UpdatedDate = ljh.UpdatedDate,
                                    }).ToListAsync();

                var deleteHotels = await (from ljh in dB_LogisticsproContext.LpJHotelAudit
                                    join j in dB_LogisticsproContext.LpJobCardAudit on ljh.JobCardId equals j.Id
                                    join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                    join v in dB_LogisticsproContext.LpMVender on ljh.VendorId equals v.Id
                                    where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                    ((userId == null || userId.Any(x => ljh.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljh.UpdatedBy.Value == x))) &&
                                    (dateFrom == null || (ljh.AuditDate.HasValue && ljh.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                    (dateTo == null || (ljh.AuditDate.HasValue && ljh.AuditDate.Value.Date <= dateTo.Value.Date)) &&
                                    j.AuditAction == "DELETE"
                                          select new JobCardHotelHistory
                                    {
                                        Id = ljh.Id == null ? 0 : ljh.Id.Value,
                                        JobCardNo = j.JobCardCode,
                                        HotelCode = ljh.HotelCode,
                                        PaxName = ljh.PaxName,
                                        CheckIn = ljh.CheckIn,
                                        CheckOut = ljh.CheckOut,
                                        Adults = ljh.Adults,
                                        Children = ljh.Children,
                                        Infants = ljh.Infants,
                                        HotelName = ljh.HotelName,
                                        IsVatIncludedCost = ljh.IsVatIncludedCost,
                                        IsVatIncludedSell = ljh.IsVatIncludedSell,
                                        CostBaseAmount = ljh.CostBaseAmount,
                                        CostTaxAmount = ljh.CostTaxAmount,
                                        SellBaseAmount = ljh.SellBaseAmount,
                                        SellTaxAmount = ljh.SellTaxAmount,
                                        Remarks = ljh.Remarks,
                                        RoomType = ljh.RoomType,
                                        HotelConfirmation = ljh.HotelConfirmation,
                                        VenderName = v.VenderName,
                                        IsPaymentVouchered = ljh.IsPaymentVouchered,
                                        IsInvoiced = ljh.IsInvoiced,
                                        AuditAction = ljh.AuditAction,
                                        AuditDate = ljh.AuditDate,
                                        CreatedBy = ljh.CreatedBy,
                                        CreatedDate = ljh.CreatedDate,
                                        UpdatedBy = ljh.UpdatedBy,
                                        UpdatedDate = ljh.UpdatedDate,
                                    }).ToListAsync();

                hotels.AddRange(deleteHotels);

                foreach (var hotel in hotels)
                {
                    hotel.CreatedByName = await GetUser(hotel.CreatedBy);
                    hotel.UpdatedByName = await GetUser(hotel.UpdatedBy);
                }

                returnItems.Hotels.AddRange(hotels.Distinct().ToList());

                var visas = await (from ljv in dB_LogisticsproContext.LpJVisaAudit
                                   join j in dB_LogisticsproContext.LpJobCard on ljv.JobCardId equals j.Id
                                   join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                            join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                            join v in dB_LogisticsproContext.LpMVender on ljv.VendorId equals v.Id
                                            where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                             ((userId == null || userId.Any(x => ljv.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljv.UpdatedBy.Value == x))) &&
                                             (dateFrom == null || (ljv.AuditDate.HasValue && ljv.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                             (dateTo == null || (ljv.AuditDate.HasValue && ljv.AuditDate.Value.Date <= dateTo.Value.Date))
                                            select new JobCardVisaHistory
                                            {
                                                Id = ljv.Id == null ? 0 : ljv.Id.Value,
                                                JobCardNo = j.JobCardCode,
                                                VisaCode = ljv.VisaCode,
                                                VisaTypeId = ljv.VisaTypeId,
                                                PaxName = ljv.PaxName,
                                                PassportNo = ljv.PassportNo,
                                                VisaTypeName = lpvt.VisaTypeName,
                                                IsVatIncludedCost = ljv.IsVatIncludedCost,
                                                IsVatIncludedSell = ljv.IsVatIncludedSell,
                                                CostBaseAmount = ljv.CostBaseAmount,
                                                CostTaxAmount = ljv.CostTaxAmount,
                                                SellBaseAmount = ljv.SellBaseAmount,
                                                SellTaxAmount = ljv.SellTaxAmount,
                                                JobCardId = ljv.JobCardId,
                                                Nationality = ljv.Nationality,
                                                Remarks = ljv.Remarks,
                                                VendorName = v.VenderName,
                                                IsPaymentVouchered = ljv.IsPaymentVouchered,
                                                IsInvoiced = ljv.IsInvoiced,
                                                AuditAction = ljv.AuditAction,
                                                AuditDate = ljv.AuditDate,
                                                CreatedBy = ljv.CreatedBy,
                                                CreatedDate = ljv.CreatedDate,
                                                UpdatedBy = ljv.UpdatedBy,
                                                UpdatedDate = ljv.UpdatedDate,
                                            }).ToListAsync();

                var deleteVisas = await (from ljv in dB_LogisticsproContext.LpJVisaAudit
                                   join j in dB_LogisticsproContext.LpJobCardAudit on ljv.JobCardId equals j.Id
                                   join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                   join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                   join v in dB_LogisticsproContext.LpMVender on ljv.VendorId equals v.Id
                                   where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                    ((userId == null || userId.Any(x => ljv.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljv.UpdatedBy.Value == x))) &&
                                    (dateFrom == null || (ljv.AuditDate.HasValue && ljv.AuditDate.Value.Date >= dateFrom.Value.Date)) &&
                                    (dateTo == null || (ljv.AuditDate.HasValue && ljv.AuditDate.Value.Date <= dateTo.Value.Date)) &&
                                    j.AuditAction == "DELETE"
                                         select new JobCardVisaHistory
                                   {
                                       Id = ljv.Id == null ? 0 : ljv.Id.Value,
                                       JobCardNo = j.JobCardCode,
                                       VisaCode = ljv.VisaCode,
                                       VisaTypeId = ljv.VisaTypeId,
                                       PaxName = ljv.PaxName,
                                       PassportNo = ljv.PassportNo,
                                       VisaTypeName = lpvt.VisaTypeName,
                                       IsVatIncludedCost = ljv.IsVatIncludedCost,
                                       IsVatIncludedSell = ljv.IsVatIncludedSell,
                                       CostBaseAmount = ljv.CostBaseAmount,
                                       CostTaxAmount = ljv.CostTaxAmount,
                                       SellBaseAmount = ljv.SellBaseAmount,
                                       SellTaxAmount = ljv.SellTaxAmount,
                                       JobCardId = ljv.JobCardId,
                                       Nationality = ljv.Nationality,
                                       Remarks = ljv.Remarks,
                                       VendorName = v.VenderName,
                                       IsPaymentVouchered = ljv.IsPaymentVouchered,
                                       IsInvoiced = ljv.IsInvoiced,
                                       AuditAction = ljv.AuditAction,
                                       AuditDate = ljv.AuditDate,
                                       CreatedBy = ljv.CreatedBy,
                                       CreatedDate = ljv.CreatedDate,
                                       UpdatedBy = ljv.UpdatedBy,
                                       UpdatedDate = ljv.UpdatedDate,
                                   }).ToListAsync();

                visas.AddRange(deleteVisas);

                foreach (var visa in visas)
                {
                    visa.CreatedByName = await GetUser(visa.CreatedBy);
                    visa.UpdatedByName = await GetUser(visa.UpdatedBy);
                }

                returnItems.Visa.AddRange(visas.Distinct().ToList());


                var miscellaneousItems = await (from ljm in dB_LogisticsproContext.LpJMiscellaneousAudit
                                                     join j in dB_LogisticsproContext.LpJobCard on ljm.JobCardId equals j.Id
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     join v in dB_LogisticsproContext.LpMVender on ljm.VendorId equals v.Id
                                                     where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                                      ((userId == null || userId.Any(x => ljm.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljm.UpdatedBy.Value == x))) &&
                                                     (dateFrom == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date >= dateFrom.Value.Date)) &&
                                                     (dateTo == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date <= dateTo.Value.Date))
                                                     select new JobCardMiscellaneousHistory
                                                     {
                                                         Id = ljm.Id == null ? 0 : ljm.Id.Value,
                                                         JobCardNo = j.JobCardCode,
                                                         MiscellaneousCode = ljm.MiscellaneousCode,
                                                         PaxName = ljm.PaxName,
                                                         PaxNumber = ljm.PaxNumber,
                                                         Description = ljm.Description,
                                                         MisDate = ljm.MisDate,
                                                         IsVatIncludedCost = ljm.IsVatIncludedCost,
                                                         IsVatIncludedSell = ljm.IsVatIncludedSell,
                                                         CostBaseAmount = ljm.CostBaseAmount,
                                                         CostTaxAmount = ljm.CostTaxAmount,
                                                         SellBaseAmount = ljm.SellBaseAmount,
                                                         SellTaxAmount = ljm.SellTaxAmount,
                                                         JobCardId = ljm.JobCardId,
                                                         Remarks = ljm.Remarks,
                                                         VendorName = v.VenderName,
                                                         IsPaymentVouchered = ljm.IsPaymentVouchered,
                                                         IsInvoiced = ljm.IsInvoiced,
                                                         IsFinance = ljm.IsFinance,
                                                         AuditAction = ljm.AuditAction,
                                                         AuditDate = ljm.AuditDate,
                                                         CreatedBy = ljm.CreatedBy,
                                                         CreatedDate = ljm.CreatedDate,
                                                         UpdatedBy = ljm.UpdatedBy,
                                                         UpdatedDate = ljm.UpdatedDate
                                                     }).ToListAsync();

                var deleteMiscellaneousItems = await (from ljm in dB_LogisticsproContext.LpJMiscellaneousAudit
                                                      join j in dB_LogisticsproContext.LpJobCardAudit on ljm.JobCardId equals j.Id
                                                      join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                      join v in dB_LogisticsproContext.LpMVender on ljm.VendorId equals v.Id
                                                      where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                                       ((userId == null || userId.Any(x => ljm.CreatedBy.Value == x)) || (userId == null || userId.Any(x => ljm.UpdatedBy.Value == x))) &&
                                                      (dateFrom == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date >= dateFrom.Value.Date)) &&
                                                      (dateTo == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date <= dateTo.Value.Date)) &&
                                                      j.AuditAction == "DELETE"
                                                      select new JobCardMiscellaneousHistory
                                                      {
                                                          Id = ljm.Id == null ? 0 : ljm.Id.Value,
                                                          JobCardNo = j.JobCardCode,
                                                          MiscellaneousCode = ljm.MiscellaneousCode,
                                                          PaxName = ljm.PaxName,
                                                          PaxNumber = ljm.PaxNumber,
                                                          Description = ljm.Description,
                                                          MisDate = ljm.MisDate,
                                                          IsVatIncludedCost = ljm.IsVatIncludedCost,
                                                          IsVatIncludedSell = ljm.IsVatIncludedSell,
                                                          CostBaseAmount = ljm.CostBaseAmount,
                                                          CostTaxAmount = ljm.CostTaxAmount,
                                                          SellBaseAmount = ljm.SellBaseAmount,
                                                          SellTaxAmount = ljm.SellTaxAmount,
                                                          JobCardId = ljm.JobCardId,
                                                          Remarks = ljm.Remarks,
                                                          VendorName = v.VenderName,
                                                          IsPaymentVouchered = ljm.IsPaymentVouchered,
                                                          IsInvoiced = ljm.IsInvoiced,
                                                          IsFinance = ljm.IsFinance,
                                                          AuditAction = ljm.AuditAction,
                                                          AuditDate = ljm.AuditDate,
                                                          CreatedBy = ljm.CreatedBy,
                                                          CreatedDate = ljm.CreatedDate,
                                                          UpdatedBy = ljm.UpdatedBy,
                                                          UpdatedDate = ljm.UpdatedDate
                                                      }).ToListAsync();

                miscellaneousItems.AddRange(deleteMiscellaneousItems);

                foreach (var miscellaneousItem in miscellaneousItems)
                {
                    miscellaneousItem.CreatedByName = await GetUser(miscellaneousItem.CreatedBy);
                    miscellaneousItem.UpdatedByName = await GetUser(miscellaneousItem.UpdatedBy);
                }

                returnItems.Miscellaneous.AddRange(miscellaneousItems.Distinct().ToList());

                return returnItems;
            }
            catch (Exception e)
            {

                throw;
            }


        }

        private async Task<string> GetUser(int? userId)
        {
            var user= await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(i => i.Id == userId);
            return user==null?string.Empty:user.UserName;
        }
        public async Task<LineGenerateItem> GetUnInvoiceReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
                var returnItems = new LineGenerateItem();

                var reportItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                         join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                         join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                         where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                                  (bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                                  (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                                  (clientRef == null || lpt.CustomerRef == clientRef) &&
                                                  (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date >= dateFrom.Value.Date)) &&
                                                  (dateTo == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date <= dateTo.Value.Date)) &&
                                                  lpt.IsBatched == false && (batchNo == null && (vendorId == null || vendorId.Count == 0))
                                                  && lpt.IsInvoiced==false
                                         select new JobCardTransportation
                                         {
                                             Id = lpt.Id,
                                             BookingRef = lpt.TransportationCode,
                                             CustomerRef = lpt.CustomerRef,
                                             Remarks = lpt.Remarks,
                                             DropoffLocation = lpt.DropoffLocation,
                                             CostBaseAmount = lpt.CostBaseAmount,
                                             CostTaxAmount = lpt.CostTaxAmount,
                                             IsVatIncludedSell = lpt.IsVatIncludedSell,
                                             IsVatIncludedCost = lpt.IsVatIncludedCost,
                                             SellBaseAmount = lpt.SellBaseAmount,
                                             SellTaxAmount = lpt.SellTaxAmount,
                                             JobCardNo = jc.JobCardCode,
                                             PickupLocation = lpt.PickupLocation,
                                             PickupTime = lpt.PickupTime,
                                             Adults = lpt.Adults,
                                             Children = lpt.Children,
                                             Infants = lpt.Infants,
                                             VehicleType = lpt.VehicleType,
                                             FlightNo = lpt.FlightNo,
                                             FlightTime = lpt.FlightTime,
                                             PaxName = lpt.PaxName,
                                             CustomerName = c.CustomerName,
                                             Parking = lpt.Parking,
                                             ParkingTaxAmount = lpt.ParkingTaxAmount,
                                             Extras = lpt.Extras,
                                             ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                             Water = lpt.Water,
                                             WaterTaxAmount = lpt.WaterTaxAmount,
                                             ParkingSell = lpt.ParkingSell,
                                             ParkingTaxAmountSell = lpt.ParkingTaxAmountSell,
                                             ExtrasSell = lpt.ExtrasSell,
                                             ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                             WaterSell = lpt.WaterSell,
                                             WaterTaxAmountSell = lpt.WaterTaxAmountSell
                                         }).OrderByDescending(i => i.Id).ToListAsync();


                var reportBatchItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                              join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                              join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                              join bl in dB_LogisticsproContext.LpLBatchLineItem on lpt.Id equals bl.ContextId
                                              join b in dB_LogisticsproContext.LpLBatch on bl.BatchId equals b.Id
                                              join v in dB_LogisticsproContext.LpMVender on b.VenderId equals v.Id
                                              where (jobCardCode == null || jc.JobCardCode.Contains(jobCardCode)) &&
                                                       (bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                                       (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                                       (clientRef == null || lpt.CustomerRef == clientRef) &&
                                                       (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date >= dateFrom.Value.Date)) &&
                                                   (dateTo == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date <= dateTo.Value.Date)) &&
                                                       (batchNo == null || b.BatchCode.Contains(batchNo)) &&
                                                           (vendorId == null || vendorId.Count == 0 || (b.VenderId != null && vendorId.Any(x => b.VenderId.Value == x))) &&
                                                          lpt.IsBatched == true 
                                                          && lpt.IsInvoiced == false
                                              select new JobCardTransportation
                                              {
                                                  Id = lpt.Id,
                                                  BookingRef = lpt.TransportationCode,
                                                  CustomerRef = lpt.CustomerRef,
                                                  Remarks = lpt.Remarks,
                                                  DropoffLocation = lpt.DropoffLocation,
                                                  CostBaseAmount = lpt.CostBaseAmount,
                                                  CostTaxAmount = lpt.CostTaxAmount,
                                                  IsVatIncludedSell = lpt.IsVatIncludedSell,
                                                  IsVatIncludedCost = lpt.IsVatIncludedCost,
                                                  SellBaseAmount = lpt.SellBaseAmount,
                                                  SellTaxAmount = lpt.SellTaxAmount,
                                                  JobCardNo = jc.JobCardCode,
                                                  PickupLocation = lpt.PickupLocation,
                                                  PickupTime = lpt.PickupTime,
                                                  Adults = lpt.Adults,
                                                  Children = lpt.Children,
                                                  Infants = lpt.Infants,
                                                  VehicleType = lpt.VehicleType,
                                                  FlightNo = lpt.FlightNo,
                                                  FlightTime = lpt.FlightTime,
                                                  PaxName = lpt.PaxName,
                                                  CustomerName = c.CustomerName,
                                                  SuplierName = v.VenderName,
                                                  BatchNo = b.BatchCode,
                                              }).OrderByDescending(i => i.Id).ToListAsync();

                reportItems.AddRange(reportBatchItems);

                returnItems.Transportations.AddRange(reportItems.Distinct().OrderBy(i => i.PickupTime).ToList());


                var batchHotelItems = await (from j in dB_LogisticsproContext.LpJobCard
                                             join ljh in dB_LogisticsproContext.LpJHotel on j.Id equals ljh.JobCardId
                                             join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                             join v in dB_LogisticsproContext.LpMVender on ljh.VendorId equals v.Id
                                             where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                              (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljh.VendorId.Value == x)) &&
                                             (dateFrom == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date >= dateFrom.Value.Date)) &&
                                             (dateTo == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date <= dateTo.Value.Date)) 
                                             && ljh.IsInvoiced == false
                                             select new JobCardHotel
                                             {
                                                 Id = ljh.Id,
                                                 JobCardNo = j.JobCardCode,
                                                 HotelCode = ljh.HotelCode,
                                                 PaxName = ljh.PaxName,
                                                 CheckIn = ljh.CheckIn,
                                                 CheckOut = ljh.CheckOut,
                                                 Adults = ljh.Adults,
                                                 Children = ljh.Children,
                                                 Infants = ljh.Infants,
                                                 HotelName = ljh.HotelName,
                                                 IsVatIncludedCost = ljh.IsVatIncludedCost,
                                                 IsVatIncludedSell = ljh.IsVatIncludedSell,
                                                 CostBaseAmount = ljh.CostBaseAmount,
                                                 CostTaxAmount = ljh.CostTaxAmount,
                                                 SellBaseAmount = ljh.SellBaseAmount,
                                                 SellTaxAmount = ljh.SellTaxAmount,
                                                 Remarks = ljh.Remarks,
                                                 RoomType = ljh.RoomType,
                                                 HotelConfirmation = ljh.HotelConfirmation,
                                                 VenderName = v.VenderName
                                             }).ToListAsync();

                returnItems.Hotels.AddRange(batchHotelItems.Distinct().OrderBy(i => i.CheckIn).ToList());

                var batchVisaItems = await (from j in dB_LogisticsproContext.LpJobCard
                                            join ljv in dB_LogisticsproContext.LpJVisa on j.Id equals ljv.JobCardId
                                            join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                            join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                            join v in dB_LogisticsproContext.LpMVender on ljv.VendorId equals v.Id
                                            where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                             (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljv.VendorId.Value == x)) &&
                                             (dateFrom == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date >= dateFrom.Value.Date)) &&
                                             (dateTo == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date <= dateTo.Value.Date))
                                             && ljv.IsInvoiced == false
                                            select new JobCardVisa
                                            {
                                                Id = ljv.Id,
                                                JobCardNo = j.JobCardCode,
                                                VisaCode = ljv.VisaCode,
                                                VisaTypeId = ljv.VisaTypeId,
                                                PaxName = ljv.PaxName,
                                                PassportNo = ljv.PassportNo,
                                                VisaTypeName = lpvt.VisaTypeName,
                                                IsVatIncludedCost = ljv.IsVatIncludedCost,
                                                IsVatIncludedSell = ljv.IsVatIncludedSell,
                                                CostBaseAmount = ljv.CostBaseAmount,
                                                CostTaxAmount = ljv.CostTaxAmount,
                                                SellBaseAmount = ljv.SellBaseAmount,
                                                SellTaxAmount = ljv.SellTaxAmount,
                                                JobCardId = ljv.JobCardId,
                                                Nationality = ljv.Nationality,
                                                Remarks = ljv.Remarks,
                                                VendorName = v.VenderName
                                            }).ToListAsync();

                returnItems.Visa.AddRange(batchVisaItems.Distinct().OrderBy(i => i.CreatedDate).ToList());


                var batchMiscellaneousItems = await (from j in dB_LogisticsproContext.LpJobCard
                                                     join ljm in dB_LogisticsproContext.LpJMiscellaneous on j.Id equals ljm.JobCardId
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     join v in dB_LogisticsproContext.LpMVender on ljm.VendorId equals v.Id
                                                     where (jobCardCode == null || j.JobCardCode.Contains(jobCardCode)) &&
                                                     (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => ljm.VendorId.Value == x)) &&
                                                     (dateFrom == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date >= dateFrom.Value.Date)) &&
                                                     (dateTo == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date <= dateTo.Value.Date))
                                                     && ljm.IsInvoiced == false
                                                     select new JobCardMiscellaneous
                                                     {
                                                         Id = ljm.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         MiscellaneousCode = ljm.MiscellaneousCode,
                                                         PaxName = ljm.PaxName,
                                                         PaxNumber = ljm.PaxNumber,
                                                         Description = ljm.Description,
                                                         MisDate = ljm.MisDate,
                                                         IsVatIncludedCost = ljm.IsVatIncludedCost,
                                                         IsVatIncludedSell = ljm.IsVatIncludedSell,
                                                         CostBaseAmount = ljm.CostBaseAmount,
                                                         CostTaxAmount = ljm.CostTaxAmount,
                                                         SellBaseAmount = ljm.SellBaseAmount,
                                                         SellTaxAmount = ljm.SellTaxAmount,
                                                         JobCardId = ljm.JobCardId,
                                                         Remarks = ljm.Remarks,
                                                         VendorName = v.VenderName
                                                     }).ToListAsync();

                returnItems.Miscellaneous.AddRange(batchMiscellaneousItems.Distinct().OrderBy(i => i.MisDate).ToList());

                return returnItems;

        }

        public async Task<List<ReportItem>> GetDailyReportItems(string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom)
        {
            var reportItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                     join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                     join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                     where ((bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                              (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                              (clientRef == null || lpt.CustomerRef == clientRef) &&
                                              (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date == dateFrom.Value.Date)) &&
                                              lpt.IsBatched == false)
                                     select new ReportItem
                                     {
                                         Id = lpt.Id,
                                         BookingRef = lpt.TransportationCode,
                                         CustomerRef = lpt.CustomerRef,
                                         Remarks = lpt.Remarks,
                                         DropoffLocation = lpt.DropoffLocation,
                                         CostBaseAmount = lpt.CostBaseAmount,
                                         CostTaxAmount = lpt.CostTaxAmount,
                                         IsVatIncludedSell = lpt.IsVatIncludedSell,
                                         IsVatIncludedCost = lpt.IsVatIncludedCost,
                                         SellBaseAmount = lpt.SellBaseAmount,
                                         SellTaxAmount = lpt.SellTaxAmount,
                                         JobCardNo = jc.JobCardCode,
                                         PickupLocation = lpt.PickupLocation,
                                         PickupTime = lpt.PickupTime,
                                         Adults = lpt.Adults,
                                         Children = lpt.Children,
                                         Infants = lpt.Infants,
                                         VehicleType = lpt.VehicleType,
                                         FlightNo = lpt.FlightNo,
                                         FlightTime = lpt.FlightTime,
                                         PaxName = lpt.PaxName,
                                         CustomerName = c.CustomerName
                                     }).OrderBy(i => i.PickupTime).ToListAsync();


            var reportBatchItems = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                          join jc in dB_LogisticsproContext.LpJobCard on lpt.JobCardId equals jc.Id
                                          join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                          join bl in dB_LogisticsproContext.LpLBatchLineItem on lpt.Id equals bl.ContextId
                                          join b in dB_LogisticsproContext.LpLBatch on bl.BatchId equals b.Id
                                          join v in dB_LogisticsproContext.LpMVender on b.VenderId equals v.Id
                                          where ((bookingRef == null || lpt.TransportationCode.Contains(bookingRef)) &&
                                                   (customerId == null || customerId.Count == 0 || customerId.Any(x => jc.CustomerId.Value == x)) &&
                                             (vendorId == null || vendorId.Count == 0 || vendorId.Any(x => b.VenderId.Value == x)) &&
                                                   (clientRef == null || lpt.CustomerRef == clientRef) &&
                                                   (dateFrom == null || (lpt.PickupTime.HasValue && lpt.PickupTime.Value.Date == dateFrom.Value.Date)) &&
                                                    (batchNo == null || b.BatchCode.Contains(batchNo)) &&
                                                      lpt.IsBatched == true)
                                          select new ReportItem
                                          {
                                              Id = lpt.Id,
                                              BookingRef = lpt.TransportationCode,
                                              CustomerRef = lpt.CustomerRef,
                                              Remarks = lpt.Remarks,
                                              DropoffLocation = lpt.DropoffLocation,
                                              CostBaseAmount = lpt.CostBaseAmount,
                                              CostTaxAmount = lpt.CostTaxAmount,
                                              IsVatIncludedSell = lpt.IsVatIncludedSell,
                                              IsVatIncludedCost = lpt.IsVatIncludedCost,
                                              SellBaseAmount = lpt.SellBaseAmount,
                                              SellTaxAmount = lpt.SellTaxAmount,
                                              JobCardNo = jc.JobCardCode,
                                              PickupLocation = lpt.PickupLocation,
                                              PickupTime = lpt.PickupTime,
                                              Adults = lpt.Adults,
                                              Children = lpt.Children,
                                              Infants = lpt.Infants,
                                              VehicleType = lpt.VehicleType,
                                              FlightNo = lpt.FlightNo,
                                              FlightTime = lpt.FlightTime,
                                              PaxName = lpt.PaxName,
                                              CustomerName = c.CustomerName,
                                              SuplierName = v.VenderName,
                                              BatchNo = b.BatchCode,
                                          }).OrderBy(i => i.PickupTime).ToListAsync();
            reportItems.AddRange(reportBatchItems);

            return reportItems.Distinct().OrderBy(i => i.PickupTime).ToList();
        }

        public async Task<bool> UpdateCost(UpdateCostRequest updateCostRequest)
        {
            try
            {
                if (updateCostRequest.Id > 0)
                {
                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == updateCostRequest.Id);
                    if (lpJTransportation != null)
                    {
                        
                        lpJTransportation.CostBaseAmount = updateCostRequest.CostBaseAmount;
                        lpJTransportation.CostTaxAmount = updateCostRequest.CostTaxAmount;
                        lpJTransportation.SellBaseAmount = updateCostRequest.SellBaseAmount;
                        lpJTransportation.SellTaxAmount = updateCostRequest.SellTaxAmount;
                        lpJTransportation.IsVatIncludedCost = updateCostRequest.IsVatIncludedCost;
                        lpJTransportation.IsVatIncludedSell = updateCostRequest.IsVatIncludedSell;
                        lpJTransportation.Parking = updateCostRequest.Parking;
                        lpJTransportation.Water = updateCostRequest.Water;
                        lpJTransportation.Extras = updateCostRequest.Extras;
                        lpJTransportation.ExtrasTaxAmount = updateCostRequest.ExtrasTaxAmount;
                        lpJTransportation.ParkingSell = updateCostRequest.ParkingSell;
                        lpJTransportation.WaterSell = updateCostRequest.WaterSell;
                        lpJTransportation.ExtrasSell = updateCostRequest.ExtrasSell;
                        lpJTransportation.ExtrasTaxAmountSell = updateCostRequest.ExtrasTaxAmountSell;
                        lpJTransportation.UpdatedBy = updateCostRequest.UserId;
                        lpJTransportation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpJTransportation).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();

                        LpJTransportationAudit transportationUpdateAudit = new LpJTransportationAudit();
                        transportationUpdateAudit.Id = lpJTransportation.Id;
                        transportationUpdateAudit.TransportationCode = lpJTransportation.TransportationCode;
                        transportationUpdateAudit.JobCardId = lpJTransportation.JobCardId;
                        transportationUpdateAudit.PaxName = lpJTransportation.PaxName;
                        transportationUpdateAudit.CustomerRef = lpJTransportation.CustomerRef;
                        transportationUpdateAudit.Remarks = lpJTransportation.Remarks;
                        transportationUpdateAudit.Adults = lpJTransportation.Adults;
                        transportationUpdateAudit.Children = lpJTransportation.Children;
                        transportationUpdateAudit.Infants = lpJTransportation.Infants;
                        transportationUpdateAudit.CostBaseAmount = lpJTransportation.CostBaseAmount;
                        transportationUpdateAudit.CostTaxAmount = lpJTransportation.CostTaxAmount;
                        transportationUpdateAudit.SellBaseAmount = lpJTransportation.SellBaseAmount;
                        transportationUpdateAudit.SellTaxAmount = lpJTransportation.SellTaxAmount;
                        transportationUpdateAudit.DropoffLocation = lpJTransportation.DropoffLocation;
                        transportationUpdateAudit.PickupLocation = lpJTransportation.PickupLocation;
                        transportationUpdateAudit.PickupTime = lpJTransportation.PickupTime;
                        transportationUpdateAudit.FlightNo = lpJTransportation.FlightNo;
                        transportationUpdateAudit.FlightTime = lpJTransportation.FlightTime;
                        transportationUpdateAudit.VehicleType = lpJTransportation.VehicleType;
                        transportationUpdateAudit.IsVatIncludedCost = lpJTransportation.IsVatIncludedCost;
                        transportationUpdateAudit.IsVatIncludedSell = lpJTransportation.IsVatIncludedSell;
                        transportationUpdateAudit.Extras = lpJTransportation.Extras;
                        transportationUpdateAudit.ExtrasTaxAmount = lpJTransportation.ExtrasTaxAmount;
                        transportationUpdateAudit.Parking = lpJTransportation.Parking;
                        transportationUpdateAudit.Water = lpJTransportation.Water;
                        transportationUpdateAudit.ExtrasSell = lpJTransportation.ExtrasSell;
                        transportationUpdateAudit.ExtrasTaxAmountSell = lpJTransportation.ExtrasTaxAmountSell;
                        transportationUpdateAudit.ParkingSell = lpJTransportation.ParkingSell;
                        transportationUpdateAudit.WaterSell = lpJTransportation.WaterSell;
                        transportationUpdateAudit.CreatedBy = lpJTransportation.CreatedBy;
                        transportationUpdateAudit.CreatedDate = lpJTransportation.CreatedDate;
                        transportationUpdateAudit.UpdatedBy = lpJTransportation.UpdatedBy;
                        transportationUpdateAudit.UpdatedDate = lpJTransportation.UpdatedDate;
                        transportationUpdateAudit.IsBatched = lpJTransportation.IsBatched;
                        transportationUpdateAudit.IsInvoiced = lpJTransportation.IsInvoiced;
                        transportationUpdateAudit.IsPaymentVouchered = lpJTransportation.IsPaymentVouchered;
                        transportationUpdateAudit.AuditAction = "UPDATE";
                        transportationUpdateAudit.AuditDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpJTransportationAudit.Add(transportationUpdateAudit);
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<List<Country>> GetCountries()
        {
            List<Country> countries = new List<Country>();
            var counrtyList = await dB_LogisticsproContext.LpMCounrty.OrderBy(i => i.CounrtyName).ToListAsync();

            counrtyList.ForEach(c =>
            {
                countries.Add(new Country
                {
                    Name = c.CounrtyName,
                    Ccn3 = c.Ccn3.Value
                });
            });


            return countries;
        }
    }   
}
