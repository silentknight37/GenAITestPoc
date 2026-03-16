using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using LogisticsPro_Common;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LogisticsPro_Data.Repository
{
    public class LogisticsRepository : ILogisticsRepository
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public LogisticsRepository(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        public async Task<List<BatchLineItem>> GetBatchJobCardTransportation(DateTime? batchDate)
        {
            try
            {
                List<BatchLineItem> batchLineItems = new List<BatchLineItem>();
                var batchItems = await (from ljt in dB_LogisticsproContext.LpJTransportation
                                        join jc in dB_LogisticsproContext.LpJobCard on ljt.JobCardId equals jc.Id
                                        join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                        where ljt.IsBatched == false &&
                                        ljt.PickupTime != null &&
                                        batchDate != null &&
                                        ljt.PickupTime.Value.Date == batchDate.Value.Date
                                        select new BatchLineItem
                                        {
                                            Id = ljt.Id,
                                            JobCardId = ljt.JobCardId,
                                            JobCardNo = jc.JobCardCode,
                                            CustomerName =c.CustomerName,
                                            BookingRef=ljt.TransportationCode,
                                            Adults= ljt.Adults,
                                            Children= ljt.Children,
                                            Infants= ljt.Infants,
                                            DropoffLocation= ljt.DropoffLocation,
                                            FlightNo= ljt.FlightNo,
                                            FlightTime = ljt.FlightTime,
                                            PaxName = ljt.PaxName,
                                            PickupLocation= ljt.PickupLocation,
                                            PickupTime= ljt.PickupTime,
                                            Remarks= ljt.Remarks,
                                            VehicleType = ljt.VehicleType
                                        }).ToListAsync();
                if(batchItems!=null && batchItems.Any() ) {
                    batchLineItems.AddRange(batchItems);
                }
                return batchLineItems.Distinct().OrderBy(i=>i.PickupTime).ToList();
            }
            catch (Exception e)
            {

                throw;
            }            
        }
        public async Task<BatchLineItemResponse> SaveBatchItem(BatchItemSaveRequest batchItemSaveRequest)
        {
            try
            {
                var isInValidList = await dB_LogisticsproContext.LpLBatchLineItem.Where(o => batchItemSaveRequest.Ids.Contains(o.ContextId.Value) && o.ContextType==(int)EnumContextType.Transpotation).ToListAsync();
                if (isInValidList.Any())
                {
                    return new BatchLineItemResponse();
                }

                var batchPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "BatchPrefix");
                var currentBatchNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentBatchNumber");
                string batchPrefixCode = string.Empty;
                int nextNumber = 0;

                if (batchPrefix != null && currentBatchNumber != null)
                {
                    nextNumber = int.Parse(currentBatchNumber.SystemValue) + 1;
                    batchPrefixCode = $"{batchPrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpLBatch lpLBatch = new LpLBatch();
                lpLBatch.BatchDate = batchItemSaveRequest.BatchDate;
                lpLBatch.VenderId = batchItemSaveRequest.VendorId;
                lpLBatch.BatchCode = batchPrefixCode;
                lpLBatch.CreatedBy = batchItemSaveRequest.UserId;
                lpLBatch.CreatedDate = DateTime.UtcNow;
                lpLBatch.UpdatedBy = batchItemSaveRequest.UserId;
                lpLBatch.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpLBatch.Add(lpLBatch);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentBatchNumber.SystemValue = nextNumber.ToString();
                currentBatchNumber.UpdatedBy= batchItemSaveRequest.UserId;
                currentBatchNumber.UpdatedDate= DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentBatchNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                foreach (var batchItem in batchItemSaveRequest.Ids)
                {
                    LpLBatchLineItem lpLBatchLineItem = new LpLBatchLineItem();
                    lpLBatchLineItem.BatchId = lpLBatch.Id;
                    lpLBatchLineItem.ContextId = batchItem;
                    lpLBatchLineItem.ContextType = (int)EnumContextType.Transpotation;
                    lpLBatchLineItem.CreatedBy = batchItemSaveRequest.UserId;
                    lpLBatchLineItem.CreatedDate = DateTime.UtcNow;
                    lpLBatchLineItem.UpdatedBy = batchItemSaveRequest.UserId;
                    lpLBatchLineItem.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpLBatchLineItem.Add(lpLBatchLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(o => o.Id == batchItem);
                    lpJTransportation.IsBatched = true;
                    lpJTransportation.UpdatedDate= DateTime.UtcNow;
                    lpJTransportation.UpdatedBy= batchItemSaveRequest.UserId;
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

                }
                var vendor = await dB_LogisticsproContext.LpMVender.FirstOrDefaultAsync(i => i.Id == lpLBatch.VenderId);
                return new BatchLineItemResponse
                {
                    Id= lpLBatch.Id,
                    BatchDate= lpLBatch.BatchDate,
                    BatchNo=lpLBatch.BatchCode,
                    SuplierName= vendor==null?"": vendor.VenderName
                };
            }
            catch (Exception e)
            {

                throw;
                return new BatchLineItemResponse();
            }
            return new BatchLineItemResponse();

        }

        public async Task<List<Batch>> GetBatches(bool isFirstLoad, string? batchCode, List<int>? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo, string? jobCardNumber)
        {
            try
            {
                int takeValue = 100000;
                if (isFirstLoad)
                {
                    takeValue = 10;
                }

                var batchList = new List<Batch>();

                if (vendorId == null || vendorId.Count == 0) {
                    batchList.AddRange(await (from lplb in dB_LogisticsproContext.LpLBatch
                                              join lpv in dB_LogisticsproContext.LpMVender on lplb.VenderId equals lpv.Id
                                              where (batchCode == null || lplb.BatchCode.Contains(batchCode)) &&
                                              (batchDateFrom == null || (lplb.BatchDate.HasValue && lplb.BatchDate.Value.Date >= batchDateFrom.Value.Date)) &&
                                              (batchDateTo == null || (lplb.BatchDate.HasValue && lplb.BatchDate.Value.Date <= batchDateTo.Value.Date))
                                              select new Batch
                                              {
                                                  Id = lplb.Id,
                                                  BatchCode = lplb.BatchCode,
                                                  VendorId = lplb.VenderId,
                                                  VendorName = lpv.VenderName,
                                                  BatchDate = lplb.BatchDate,
                                              }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());
                }
                else {
                    var filterBatchList= new List<Batch>();
                    foreach (var vId in vendorId)
                    {
                        filterBatchList.AddRange(await (from lplb in dB_LogisticsproContext.LpLBatch
                                                  join lpv in dB_LogisticsproContext.LpMVender on lplb.VenderId equals lpv.Id
                                                  where (batchCode == null || lplb.BatchCode.Contains(batchCode)) &&
                                                  (vId == lplb.VenderId.Value) &&
                                                  (batchDateFrom == null || (lplb.BatchDate.HasValue && lplb.BatchDate.Value.Date >= batchDateFrom.Value.Date)) &&
                                                  (batchDateTo == null || (lplb.BatchDate.HasValue && lplb.BatchDate.Value.Date <= batchDateTo.Value.Date))
                                                  select new Batch
                                                  {
                                                      Id = lplb.Id,
                                                      BatchCode = lplb.BatchCode,
                                                      VendorId = lplb.VenderId,
                                                      VendorName = lpv.VenderName,
                                                      BatchDate = lplb.BatchDate,
                                                  }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());
                    }
                    var orderList = filterBatchList.OrderByDescending(i => i.Id).ToList();
                    batchList.AddRange(orderList);
                }

               
                foreach (var batch in batchList)
                {
                    var jobCardIds= await (from bi in dB_LogisticsproContext.LpLBatchLineItem
                                         join t in dB_LogisticsproContext.LpJTransportation on bi.ContextId equals t.Id
                                         join j in dB_LogisticsproContext.LpJobCard on t.JobCardId equals j.Id
                                         where bi.BatchId == batch.Id &&
                                         (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                           select new
                                           {
                                               j.Id
                                           }).Distinct().ToListAsync();
                    if (!jobCardIds.Any())
                    {
                        continue;
                    }

                    var transpostBatchItems = await(from  bi in dB_LogisticsproContext.LpLBatchLineItem
                                              join t in dB_LogisticsproContext.LpJTransportation on bi.ContextId equals t.Id
                                              where bi.BatchId== batch.Id
                                              select new BatchItem
                                              {
                                                  Id = bi.Id,
                                                  BatchId = bi.BatchId,
                                                  ContextId = bi.ContextId,
                                                  ContextType = bi.ContextType,
                                                  ContextName = ((EnumContextType)bi.ContextType).ToString(),
                                                  Code=t.TransportationCode,
                                                  Date=t.PickupTime,
                                                  Pax=t.PaxName
                                              }).Distinct().OrderBy(i => i.Date).ToListAsync();
                        

                    batch.BatchItems.AddRange(transpostBatchItems);

                    List<BatchLineItem> batchLineItems = new List<BatchLineItem>();
                    var batchItems = await (from bi in dB_LogisticsproContext.LpLBatchLineItem
                                            join ljt in dB_LogisticsproContext.LpJTransportation on bi.ContextId equals ljt.Id
                                            join jc in dB_LogisticsproContext.LpJobCard on ljt.JobCardId equals jc.Id
                                            join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                            where bi.BatchId == batch.Id
                                            select new BatchLineItem
                                            {
                                                Id = bi.Id,
                                                JobCardId = ljt.JobCardId,
                                                JobCardNo = jc.JobCardCode,
                                                CustomerName = c.CustomerName,
                                                BookingRef = ljt.TransportationCode,
                                                Adults = ljt.Adults,
                                                Children = ljt.Children,
                                                Infants = ljt.Infants,
                                                DropoffLocation = ljt.DropoffLocation,
                                                FlightNo = ljt.FlightNo,
                                                FlightTime = ljt.FlightTime,
                                                PaxName = ljt.PaxName,
                                                PickupLocation = ljt.PickupLocation,
                                                PickupTime = ljt.PickupTime,
                                                Remarks = ljt.Remarks,
                                                VehicleType = ljt.VehicleType
                                            }).Distinct().OrderBy(i => i.PickupTime).ToListAsync();
                    if (batchItems != null && batchItems.Any())
                    {
                        batchLineItems.AddRange(batchItems);
                    }

                    batch.BatchTransportItems.AddRange(batchLineItems);

                }
                var returnList = batchList.Where(i => i.BatchTransportItems.Any() || i.BatchItems.Any()).ToList();
                return returnList;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> RemoveBatchItemFromList(int id)
        {
            var lpLBatchLineItem = await dB_LogisticsproContext.LpLBatchLineItem.FirstOrDefaultAsync(i => i.Id == id);
            if(lpLBatchLineItem == null)
            {
                return false;
            }

            var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == lpLBatchLineItem.ContextId);
            if (lpJTransportation != null)
            {
                lpJTransportation.IsBatched = false;
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
            }
            dB_LogisticsproContext.LpLBatchLineItem.Remove(lpLBatchLineItem);
            await dB_LogisticsproContext.SaveChangesAsync();
            
            
            return true;
        }

        public async Task<LineGenerateItem> GetBatchJobCardTransportationByVendorId(int vendorId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber)
        {
            try
            {
                LineGenerateItem paymentVoucherGenerateItem = new LineGenerateItem();

                List<JobCardTransportation> batchLineItems = new List<JobCardTransportation>();
                var batchItems = await (from b in dB_LogisticsproContext.LpLBatch
                                        join bl in dB_LogisticsproContext.LpLBatchLineItem on b.Id equals bl.BatchId
                                        join ljt in dB_LogisticsproContext.LpJTransportation on bl.ContextId equals ljt.Id
                                        join jc in dB_LogisticsproContext.LpJobCard on ljt.JobCardId equals jc.Id
                                        join c in dB_LogisticsproContext.LpMCustomer on jc.CustomerId equals c.Id
                                        where ljt.IsBatched == true && 
                                        ljt.IsPaymentVouchered==false &&
                                        bl.ContextType == (int)EnumContextType.Transpotation &&
                                        b.VenderId== vendorId &&
                                        (fromDate==null || (ljt.PickupTime.HasValue && ljt.PickupTime.Value.Date>= fromDate.Value.Date)) &&
                                        (toDate == null || (ljt.PickupTime.HasValue && ljt.PickupTime.Value.Date<= toDate.Value.Date))&&
                                        (jobCardNumber == null || jc.JobCardCode.Contains(jobCardNumber))
                                        select new JobCardTransportation
                                        {
                                            Id = ljt.Id,
                                            BatchNo=b.BatchCode,
                                            JobCardId = ljt.JobCardId,
                                            JobCardNo = jc.JobCardCode,
                                            JobCardDescription=jc.JobDescription,
                                            CustomerName = c.CustomerName,
                                            BookingRef = ljt.TransportationCode,
                                            Adults = ljt.Adults,
                                            Children = ljt.Children,
                                            Infants = ljt.Infants,
                                            DropoffLocation = ljt.DropoffLocation,
                                            FlightNo = ljt.FlightNo,
                                            FlightTime = ljt.FlightTime,
                                            PaxName = ljt.PaxName,
                                            PickupLocation = ljt.PickupLocation,
                                            PickupTime = ljt.PickupTime,
                                            Remarks = ljt.Remarks,
                                            VehicleType = ljt.VehicleType,
                                            CostBaseAmount= ljt.CostBaseAmount,
                                            CostTaxAmount= ljt.CostTaxAmount,
                                            Extras= ljt.Extras,
                                            ExtrasTaxAmount= ljt.ExtrasTaxAmount,
                                            Parking = ljt.Parking,
                                            ParkingTaxAmount= ljt.ParkingTaxAmount,
                                            Water = ljt.Water,
                                            WaterTaxAmount= ljt.WaterTaxAmount
                                        }).ToListAsync();
                if (batchItems != null && batchItems.Any())
                {
                    paymentVoucherGenerateItem.Transportations = batchItems;
                }


                var hotel = await (from h in dB_LogisticsproContext.LpJHotel
                                   join jc in dB_LogisticsproContext.LpJobCard on h.JobCardId equals jc.Id
                                   where h.VendorId == vendorId &&
                                    h.IsPaymentVouchered == false &&
                                   (fromDate == null || (h.CheckIn.HasValue && h.CheckIn.Value.Date >= fromDate.Value.Date)) &&
                                   (toDate == null || (h.CheckIn.HasValue && h.CheckIn.Value.Date <= toDate.Value.Date)) &&
                                   (jobCardNumber == null || jc.JobCardCode.Contains(jobCardNumber))
                                   select new JobCardHotel
                                   {
                                       Id= h.Id,
                                       JobCardNo=jc.JobCardCode,
                                       JobCardDescription=jc.JobDescription,
                                       HotelCode = h.HotelCode,
                                       PaxName = h.PaxName,
                                       CheckIn = h.CheckIn,
                                       CheckOut = h.CheckOut,
                                       Adults = h.Adults,
                                       Children = h.Children,
                                       Infants = h.Infants,
                                       HotelName = h.HotelName,
                                       CostBaseAmount = h.CostBaseAmount,
                                       CostTaxAmount = h.CostTaxAmount,
                                       SellBaseAmount = h.SellBaseAmount,
                                       SellTaxAmount = h.SellTaxAmount,
                                       Remarks = h.Remarks,
                                       RoomType = h.RoomType,
                                       HotelConfirmation = h.HotelConfirmation
                                   }).ToListAsync();
                if (hotel != null && hotel.Any())
                {
                    paymentVoucherGenerateItem.Hotels = hotel;
                }

                var visa = await (from lpv in dB_LogisticsproContext.LpJVisa
                                  join jc in dB_LogisticsproContext.LpJobCard on lpv.JobCardId equals jc.Id
                                  join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                  where lpv.VendorId == vendorId &&
                                  lpv.IsPaymentVouchered == false &&
                                  (fromDate == null || (lpv.CreatedDate.HasValue && lpv.CreatedDate.Value.Date >= fromDate.Value.Date)) &&
                                  (toDate == null || (lpv.CreatedDate.HasValue && lpv.CreatedDate.Value.Date <= toDate.Value.Date)) &&
                                  (jobCardNumber == null || jc.JobCardCode.Contains(jobCardNumber))
                                  select new JobCardVisa
                                  {
                                      Id = lpv.Id,
                                      JobCardNo = jc.JobCardCode,
                                      JobCardDescription=jc.JobDescription,
                                      VisaCode = lpv.VisaCode,
                                      VisaTypeId = lpv.VisaTypeId,
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
                                      Remarks = lpv.Remarks
                                  }).ToListAsync();

                if (visa != null && visa.Any())
                {
                    paymentVoucherGenerateItem.Visa = visa;
                }

                var miscellaneous = await (from lpm in dB_LogisticsproContext.LpJMiscellaneous
                                           join jc in dB_LogisticsproContext.LpJobCard on lpm.JobCardId equals jc.Id
                                           where lpm.VendorId == vendorId &&
                                                lpm.IsPaymentVouchered == false &&
                                  (fromDate == null || (lpm.MisDate.HasValue && lpm.MisDate.Value.Date >= fromDate.Value.Date)) &&
                                  (toDate == null || (lpm.MisDate.HasValue && lpm.MisDate.Value.Date <= toDate.Value.Date)) &&
                                        (jobCardNumber == null || jc.JobCardCode.Contains(jobCardNumber))
                                           select new JobCardMiscellaneous
                                                {
                                                    Id = lpm.Id,
                                               JobCardNo = jc.JobCardCode,
                                               JobCardDescription=jc.JobDescription,
                                               MiscellaneousCode = lpm.MiscellaneousCode,
                                                    PaxName = lpm.PaxName,
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
                                                    Remarks = lpm.Remarks
                                                }).OrderByDescending(i => i.Id).ToListAsync();
                if (miscellaneous != null && miscellaneous.Any())
                {
                    paymentVoucherGenerateItem.Miscellaneous = miscellaneous;
                }

                return paymentVoucherGenerateItem;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> SavePaymentVoucher(PaymentVoucherSaveRequest paymentVoucherSaveRequest)
        {
            try
            {
                var paymentVoucherPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "PaymentVoucherPrefix");
                var currentPaymentVoucherNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentPaymentVoucherNumber");
                string paymentVoucherPrefixCode = string.Empty;
                int nextNumber = 0;

                if (paymentVoucherPrefix != null && currentPaymentVoucherNumber != null)
                {
                    nextNumber = int.Parse(currentPaymentVoucherNumber.SystemValue) + 1;
                    paymentVoucherPrefixCode = $"{paymentVoucherPrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpFPaymentVoucher lpFPaymentVoucher = new LpFPaymentVoucher();
                lpFPaymentVoucher.PaymentVoucherCode = paymentVoucherPrefixCode;
                lpFPaymentVoucher.VendorId = paymentVoucherSaveRequest.VendorId;
                lpFPaymentVoucher.VoucherAmount = paymentVoucherSaveRequest.PaymentVoucherAmount;
                lpFPaymentVoucher.VoucherDate = paymentVoucherSaveRequest.PaymentVoucherDate;
                lpFPaymentVoucher.InvoiceNo = paymentVoucherSaveRequest.Invoice;
                lpFPaymentVoucher.Remark = paymentVoucherSaveRequest.Remarks;
                lpFPaymentVoucher.CreatedBy = paymentVoucherSaveRequest.UserId;
                lpFPaymentVoucher.CreatedDate = DateTime.UtcNow;
                lpFPaymentVoucher.UpdatedBy = paymentVoucherSaveRequest.UserId;
                lpFPaymentVoucher.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpFPaymentVoucher.Add(lpFPaymentVoucher);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentPaymentVoucherNumber.SystemValue = nextNumber.ToString();
                currentPaymentVoucherNumber.UpdatedDate= DateTime.UtcNow;
                currentPaymentVoucherNumber.UpdatedBy = paymentVoucherSaveRequest.UserId;
                dB_LogisticsproContext.Entry(currentPaymentVoucherNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                foreach (var paymentVoucherItemId in paymentVoucherSaveRequest.TransportationIds)
                {
                    LpFPaymentVoucherLineItem lpFPaymentVoucherLineItem = new LpFPaymentVoucherLineItem();
                    lpFPaymentVoucherLineItem.PaymentVoucherId = lpFPaymentVoucher.Id;
                    lpFPaymentVoucherLineItem.ContextId = paymentVoucherItemId;
                    lpFPaymentVoucherLineItem.ContextTypeId = (int)EnumContextType.Transpotation;
                    lpFPaymentVoucherLineItem.CreatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.CreatedDate = DateTime.UtcNow;
                    lpFPaymentVoucherLineItem.UpdatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Add(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(o => o.Id == paymentVoucherItemId);
                    lpJTransportation.IsPaymentVouchered = true;
                    lpJTransportation.UpdatedBy = paymentVoucherSaveRequest.UserId;
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

                }

                foreach (var paymentVoucherItemId in paymentVoucherSaveRequest.HotelIds)
                {
                    LpFPaymentVoucherLineItem lpFPaymentVoucherLineItem = new LpFPaymentVoucherLineItem();
                    lpFPaymentVoucherLineItem.PaymentVoucherId = lpFPaymentVoucher.Id;
                    lpFPaymentVoucherLineItem.ContextId = paymentVoucherItemId;
                    lpFPaymentVoucherLineItem.ContextTypeId = (int)EnumContextType.Hotel;
                    lpFPaymentVoucherLineItem.CreatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.CreatedDate = DateTime.UtcNow;
                    lpFPaymentVoucherLineItem.UpdatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Add(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(o => o.Id == paymentVoucherItemId);
                    lpJHotel.IsPaymentVouchered = true;
                    lpJHotel.UpdatedBy = paymentVoucherSaveRequest.UserId;
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

                }

                foreach (var paymentVoucherItemId in paymentVoucherSaveRequest.VisaIds)
                {
                    LpFPaymentVoucherLineItem lpFPaymentVoucherLineItem = new LpFPaymentVoucherLineItem();
                    lpFPaymentVoucherLineItem.PaymentVoucherId = lpFPaymentVoucher.Id;
                    lpFPaymentVoucherLineItem.ContextId = paymentVoucherItemId;
                    lpFPaymentVoucherLineItem.ContextTypeId = (int)EnumContextType.Visa;
                    lpFPaymentVoucherLineItem.CreatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.CreatedDate = DateTime.UtcNow;
                    lpFPaymentVoucherLineItem.UpdatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Add(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(o => o.Id == paymentVoucherItemId);
                    lpJVisa.IsPaymentVouchered = true;
                    lpJVisa.UpdatedBy = paymentVoucherSaveRequest.UserId;
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

                }

                foreach (var paymentVoucherItemId in paymentVoucherSaveRequest.MiscellaneousIds)
                {
                    LpFPaymentVoucherLineItem lpFPaymentVoucherLineItem = new LpFPaymentVoucherLineItem();
                    lpFPaymentVoucherLineItem.PaymentVoucherId = lpFPaymentVoucher.Id;
                    lpFPaymentVoucherLineItem.ContextId = paymentVoucherItemId;
                    lpFPaymentVoucherLineItem.ContextTypeId = (int)EnumContextType.Miscellaneous;
                    lpFPaymentVoucherLineItem.CreatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.CreatedDate = DateTime.UtcNow;
                    lpFPaymentVoucherLineItem.UpdatedBy = paymentVoucherSaveRequest.UserId;
                    lpFPaymentVoucherLineItem.UpdatedDate = DateTime.UtcNow;

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Add(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();

                    var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(o => o.Id == paymentVoucherItemId);
                    lpJMiscellaneous.IsPaymentVouchered = true;
                    lpJMiscellaneous.UpdatedBy = paymentVoucherSaveRequest.UserId;
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

                }
                return lpFPaymentVoucher.Id;
            }
            catch (Exception e)
            {

                throw;
                return 0;
            }
            return 0;

        }

        public async Task<List<PaymentVoucher>> GetPaymentVouchers(bool isFirstLoad,string? paymentVoucherCode, string? invoiceNo, List<int>? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo, string? jobCardNumber)
        {
            try
            {
                int takeValue = 100000;
                if (isFirstLoad)
                {
                    takeValue = 10;
                }
                var paymentVoucherList = new List<PaymentVoucher>();
                if (vendorId == null || vendorId.Count == 0)
                {
                    paymentVoucherList.AddRange( await (from pv in dB_LogisticsproContext.LpFPaymentVoucher
                                                    join lpv in dB_LogisticsproContext.LpMVender on pv.VendorId equals lpv.Id
                                                    where (paymentVoucherCode == null || pv.PaymentVoucherCode.Contains(paymentVoucherCode)) &&
                                                    (invoiceNo == null || pv.InvoiceNo.Contains(invoiceNo)) &&
                                                    (paymentVoucherDateFrom == null || pv.VoucherDate >= paymentVoucherDateFrom) &&
                                                    (paymentVoucherDateTo == null || pv.VoucherDate <= paymentVoucherDateTo)
                                                    select new PaymentVoucher
                                                    {
                                                        Id = pv.Id,
                                                        PaymentVoucherCode = pv.PaymentVoucherCode,
                                                        VendorId = pv.VendorId,
                                                        VendorName = lpv.VenderName,
                                                        VendorAddressLine1 = lpv.Address1 == null || lpv.Address1 == "" ? lpv.Address2 : $"{lpv.Address1}, {lpv.Address2} ",
                                                        VendorAddressLine2 = lpv.City == null || lpv.City == "" ? lpv.CountryCode : $"{lpv.City}, {lpv.CountryCode} ",
                                                        VendorBankCode = lpv.BankCode,
                                                        VendorBankName = lpv.BankName,
                                                        PaymentVoucherDate = pv.VoucherDate,
                                                        InvoiceNo = pv.InvoiceNo,
                                                        PaymentVoucherAmount = pv.VoucherAmount,
                                                        Remark = pv.Remark,
                                                    }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());
                }

            else
            {
                var filterPaymentVoucherList = new List<PaymentVoucher>();
                foreach (var vId in vendorId)
                {
                        filterPaymentVoucherList.AddRange(await (from pv in dB_LogisticsproContext.LpFPaymentVoucher
                                                                 join lpv in dB_LogisticsproContext.LpMVender on pv.VendorId equals lpv.Id
                                                                 where (paymentVoucherCode == null || pv.PaymentVoucherCode.Contains(paymentVoucherCode)) &&
                                                                 (invoiceNo == null || pv.InvoiceNo.Contains(invoiceNo)) &&
                                                                 (pv.VendorId== vId) &&
                                                                 (paymentVoucherDateFrom == null || pv.VoucherDate >= paymentVoucherDateFrom) &&
                                                                 (paymentVoucherDateTo == null || pv.VoucherDate <= paymentVoucherDateTo)
                                                                 select new PaymentVoucher
                                                                 {
                                                                     Id = pv.Id,
                                                                     PaymentVoucherCode = pv.PaymentVoucherCode,
                                                                     VendorId = pv.VendorId,
                                                                     VendorName = lpv.VenderName,
                                                                     VendorAddressLine1 = lpv.Address1 == null || lpv.Address1 == "" ? lpv.Address2 : $"{lpv.Address1}, {lpv.Address2} ",
                                                                     VendorAddressLine2 = lpv.City == null || lpv.City == "" ? lpv.CountryCode : $"{lpv.City}, {lpv.CountryCode} ",
                                                                     VendorBankCode = lpv.BankCode,
                                                                     VendorBankName = lpv.BankName,
                                                                     PaymentVoucherDate = pv.VoucherDate,
                                                                     InvoiceNo = pv.InvoiceNo,
                                                                     PaymentVoucherAmount = pv.VoucherAmount,
                                                                     Remark = pv.Remark,
                                                                 }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync());
                    }

                    var orderList = filterPaymentVoucherList.OrderByDescending(i => i.Id).ToList();
                    paymentVoucherList.AddRange(orderList);
                }

               

            foreach (var paymentVoucher in paymentVoucherList)
                {
                    var transpostBatchItems = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                                     join t in dB_LogisticsproContext.LpJTransportation on pvi.ContextId equals t.Id
                                                     join j in dB_LogisticsproContext.LpJobCard on t.JobCardId equals j.Id
                                                     join bli in dB_LogisticsproContext.LpLBatchLineItem on t.Id equals bli.ContextId
                                                     join b in dB_LogisticsproContext.LpLBatch on bli.BatchId equals b.Id
                                                     where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Transpotation &&
                                                     (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                     select new JobCardTransportation
                                                     {
                                                         Id = pvi.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         BatchNo = b.BatchCode,
                                                         Adults = t.Adults,
                                                         Children = t.Children,
                                                         Infants = t.Infants,
                                                         BookingRef = t.TransportationCode,
                                                         CostBaseAmount = t.CostBaseAmount,
                                                         CostTaxAmount = t.CostTaxAmount,
                                                         Parking = t.Parking,
                                                         ParkingTaxAmount = t.ParkingTaxAmount,
                                                         Extras = t.Extras,
                                                         Water = t.Water,
                                                         WaterTaxAmount=t.WaterTaxAmount,
                                                         ExtrasTaxAmount = t.ExtrasTaxAmount,
                                                         PaxName = t.PaxName,
                                                         PickupTime = t.PickupTime,
                                                         VehicleType = t.VehicleType,
                                                         PickupLocation = t.PickupLocation,
                                                         DropoffLocation = t.DropoffLocation,
                                                         Remarks = t.Remarks,
                                                         FlightNo = t.FlightNo,
                                                         FlightTime = t.FlightTime
                                                     }).Distinct().OrderBy(i => i.PickupTime).Distinct().ToListAsync();


                    paymentVoucher.PaymentVoucherGenerateItem.Transportations.AddRange(transpostBatchItems);

                    var hotel = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                       join h in dB_LogisticsproContext.LpJHotel on pvi.ContextId equals h.Id
                                       join j in dB_LogisticsproContext.LpJobCard on h.JobCardId equals j.Id
                                       where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Hotel &&
                                       (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                       select new JobCardHotel
                                       {
                                           Id = pvi.Id,
                                           JobCardNo = j.JobCardCode,
                                           HotelCode = h.HotelCode,
                                           PaxName = h.PaxName,
                                           CheckIn = h.CheckIn,
                                           CheckOut = h.CheckOut,
                                           Adults = h.Adults,
                                           Children = h.Children,
                                           Infants = h.Infants,
                                           HotelName = h.HotelName,
                                           CostBaseAmount = h.CostBaseAmount,
                                           CostTaxAmount = h.CostTaxAmount,
                                           SellBaseAmount = h.SellBaseAmount,
                                           SellTaxAmount = h.SellTaxAmount,
                                           Remarks = h.Remarks,
                                           RoomType = h.RoomType,
                                           HotelConfirmation = h.HotelConfirmation
                                       }).ToListAsync();
                    paymentVoucher.PaymentVoucherGenerateItem.Hotels.AddRange(hotel);


                    var visa = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                      join lpv in dB_LogisticsproContext.LpJVisa on pvi.ContextId equals lpv.Id
                                      join j in dB_LogisticsproContext.LpJobCard on lpv.JobCardId equals j.Id
                                      join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                      where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Visa &&
                                      (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                      select new JobCardVisa
                                      {
                                          Id = pvi.Id,
                                          JobCardNo = j.JobCardCode,
                                          VisaCode = lpv.VisaCode,
                                          VisaTypeId = lpv.VisaTypeId,
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
                                          Remarks = lpv.Remarks
                                      }).ToListAsync();

                    paymentVoucher.PaymentVoucherGenerateItem.Visa.AddRange(visa);

                    var miscellaneous = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                               join lpm in dB_LogisticsproContext.LpJMiscellaneous on pvi.ContextId equals lpm.Id
                                               join j in dB_LogisticsproContext.LpJobCard on lpm.JobCardId equals j.Id
                                               where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Miscellaneous 
                                               && (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                               select new JobCardMiscellaneous
                                               {
                                                   Id = pvi.Id,
                                                   JobCardNo = j.JobCardCode,
                                                   MiscellaneousCode = lpm.MiscellaneousCode,
                                                   PaxName = lpm.PaxName,
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
                                                   Remarks = lpm.Remarks
                                               }).OrderByDescending(i => i.Id).ToListAsync();

                    paymentVoucher.PaymentVoucherGenerateItem.Miscellaneous.AddRange(miscellaneous);


                }
                var returnList= paymentVoucherList.Where(i=>i.PaymentVoucherGenerateItem.Transportations.Any() || i.PaymentVoucherGenerateItem.Hotels.Any() || i.PaymentVoucherGenerateItem.Visa.Any() || i.PaymentVoucherGenerateItem.Miscellaneous.Any()).ToList();
                return returnList;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<PaymentVoucher> GetPaymentVoucherById(int id)
        {
            try
            {
                var paymentVoucher = await (from pv in dB_LogisticsproContext.LpFPaymentVoucher
                                                join lpv in dB_LogisticsproContext.LpMVender on pv.VendorId equals lpv.Id
                                                where pv.Id== id
                                                select new PaymentVoucher
                                                {
                                                    Id = pv.Id,
                                                    PaymentVoucherCode = pv.PaymentVoucherCode,
                                                    VendorId = pv.VendorId,
                                                    VendorName = lpv.VenderName,
                                                    VendorAddressLine1 = lpv.Address1 == null || lpv.Address1 == "" ? lpv.Address2 : $"{lpv.Address1}, {lpv.Address2} ",
                                                    VendorAddressLine2 = lpv.City == null || lpv.City == "" ? lpv.CountryCode : $"{lpv.City}, {lpv.CountryCode} ",
                                                    VendorBankCode = lpv.BankCode,
                                                    VendorBankName = lpv.BankName,
                                                    PaymentVoucherDate = pv.VoucherDate,
                                                    InvoiceNo = pv.InvoiceNo,
                                                    PaymentVoucherAmount = pv.VoucherAmount,
                                                    Remark = pv.Remark,
                                                }).OrderByDescending(i => i.Id).FirstOrDefaultAsync();
                if(paymentVoucher!=null)
                {
                    var transpostBatchItems = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                                     join t in dB_LogisticsproContext.LpJTransportation on pvi.ContextId equals t.Id
                                                     join j in dB_LogisticsproContext.LpJobCard on t.JobCardId equals j.Id
                                                     join bli in dB_LogisticsproContext.LpLBatchLineItem on t.Id equals bli.ContextId
                                                     join b in dB_LogisticsproContext.LpLBatch on bli.BatchId equals b.Id
                                                     where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Transpotation
                                                     select new JobCardTransportation
                                                     {
                                                         Id = pvi.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         BatchNo = b.BatchCode,
                                                         Adults = t.Adults,
                                                         Children = t.Children,
                                                         Infants = t.Infants,
                                                         BookingRef = t.TransportationCode,
                                                         CostBaseAmount = t.CostBaseAmount,
                                                         CostTaxAmount = t.CostTaxAmount,
                                                         Parking=t.Parking,
                                                         ParkingTaxAmount = t.ParkingTaxAmount,
                                                         Extras=t.Extras,
                                                         Water=t.Water,
                                                         WaterTaxAmount=t.WaterTaxAmount,
                                                         ExtrasTaxAmount=t.ExtrasTaxAmount,
                                                         PaxName = t.PaxName,
                                                         PickupTime = t.PickupTime,
                                                         VehicleType = t.VehicleType,
                                                         PickupLocation = t.PickupLocation,
                                                         DropoffLocation = t.DropoffLocation,
                                                         Remarks = t.Remarks,
                                                         FlightNo = t.FlightNo,
                                                         FlightTime = t.FlightTime
                                                     }).Distinct().OrderBy(i => i.PickupTime).ToListAsync();


                    paymentVoucher.PaymentVoucherGenerateItem.Transportations.AddRange(transpostBatchItems);

                    var hotel = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                       join h in dB_LogisticsproContext.LpJHotel on pvi.ContextId equals h.Id
                                       join j in dB_LogisticsproContext.LpJobCard on h.JobCardId equals j.Id
                                       where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Hotel
                                       select new JobCardHotel
                                       {
                                           Id = pvi.Id,
                                           JobCardNo = j.JobCardCode,
                                           HotelCode = h.HotelCode,
                                           PaxName = h.PaxName,
                                           CheckIn = h.CheckIn,
                                           CheckOut = h.CheckOut,
                                           Adults = h.Adults,
                                           Children = h.Children,
                                           Infants = h.Infants,
                                           HotelName = h.HotelName,
                                           CostBaseAmount = h.CostBaseAmount,
                                           CostTaxAmount = h.CostTaxAmount,
                                           SellBaseAmount = h.SellBaseAmount,
                                           SellTaxAmount = h.SellTaxAmount,
                                           Remarks = h.Remarks,
                                           RoomType = h.RoomType,
                                           HotelConfirmation = h.HotelConfirmation
                                       }).ToListAsync();
                    paymentVoucher.PaymentVoucherGenerateItem.Hotels.AddRange(hotel);


                    var visa = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                      join lpv in dB_LogisticsproContext.LpJVisa on pvi.ContextId equals lpv.Id
                                      join j in dB_LogisticsproContext.LpJobCard on lpv.JobCardId equals j.Id
                                      join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                      where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Visa
                                      select new JobCardVisa
                                      {
                                          Id = pvi.Id,
                                          JobCardNo = j.JobCardCode,
                                          VisaCode = lpv.VisaCode,
                                          VisaTypeId = lpv.VisaTypeId,
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
                                          Remarks = lpv.Remarks
                                      }).ToListAsync();

                    paymentVoucher.PaymentVoucherGenerateItem.Visa.AddRange(visa);

                    var miscellaneous = await (from pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem
                                               join lpm in dB_LogisticsproContext.LpJMiscellaneous on pvi.ContextId equals lpm.Id
                                               join j in dB_LogisticsproContext.LpJobCard on lpm.JobCardId equals j.Id
                                               where pvi.PaymentVoucherId == paymentVoucher.Id && pvi.ContextTypeId == (int)EnumContextType.Miscellaneous
                                               select new JobCardMiscellaneous
                                               {
                                                   Id = pvi.Id,
                                                   JobCardNo = j.JobCardCode,
                                                   MiscellaneousCode = lpm.MiscellaneousCode,
                                                   PaxName = lpm.PaxName,
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
                                                   Remarks = lpm.Remarks
                                               }).OrderByDescending(i => i.Id).ToListAsync();

                    paymentVoucher.PaymentVoucherGenerateItem.Miscellaneous.AddRange(miscellaneous);


                }
                return paymentVoucher;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> RemovePaymentVoucherItemFromList(int id, string itemType)
        {
            var lpFPaymentVoucherLineItem = await dB_LogisticsproContext.LpFPaymentVoucherLineItem.FirstOrDefaultAsync(i => i.Id == id);

            if(lpFPaymentVoucherLineItem == null)
            {
                return false;
            }

            if (itemType == "Transportation")
            {
                var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.ContextId);
                if (lpJTransportation != null)
                {
                    lpJTransportation.IsPaymentVouchered = false;
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

                    var paymentVoucher = await dB_LogisticsproContext.LpFPaymentVoucher.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.PaymentVoucherId);
                    if (paymentVoucher != null)
                    {
                        paymentVoucher.VoucherAmount = paymentVoucher.VoucherAmount - (lpJTransportation.CostBaseAmount + lpJTransportation.CostTaxAmount + lpJTransportation.Parking + lpJTransportation.Water + lpJTransportation.Extras + lpJTransportation.ExtrasTaxAmount);
                        dB_LogisticsproContext.Entry(paymentVoucher).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Remove(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }

            if(itemType == "Hotel")
            {
                var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.ContextId);
                if (lpJHotel != null)
                {
                    lpJHotel.IsPaymentVouchered = false;
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

                    var paymentVoucher = await dB_LogisticsproContext.LpFPaymentVoucher.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.PaymentVoucherId);
                    if (paymentVoucher != null)
                    {
                        paymentVoucher.VoucherAmount = paymentVoucher.VoucherAmount - (lpJHotel.CostBaseAmount + lpJHotel.CostTaxAmount);
                        dB_LogisticsproContext.Entry(paymentVoucher).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Remove(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }

            if (itemType == "Visa")
            {
                var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.ContextId);
                if (lpJVisa != null)
                {
                    lpJVisa.IsPaymentVouchered = false;
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

                    var paymentVoucher = await dB_LogisticsproContext.LpFPaymentVoucher.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.PaymentVoucherId);
                    if (paymentVoucher != null)
                    {
                        paymentVoucher.VoucherAmount = paymentVoucher.VoucherAmount - (lpJVisa.CostBaseAmount + lpJVisa.CostTaxAmount);
                        dB_LogisticsproContext.Entry(paymentVoucher).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Remove(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }



            if (itemType == "Miscellaneous")
            {
                var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.ContextId);
                if (lpJMiscellaneous != null)
                {
                    lpJMiscellaneous.IsPaymentVouchered = false;
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

                    var paymentVoucher = await dB_LogisticsproContext.LpFPaymentVoucher.FirstOrDefaultAsync(i => i.Id == lpFPaymentVoucherLineItem.PaymentVoucherId);
                    if (paymentVoucher != null)
                    {
                        paymentVoucher.VoucherAmount = paymentVoucher.VoucherAmount - (lpJMiscellaneous.CostBaseAmount + lpJMiscellaneous.CostTaxAmount);
                        dB_LogisticsproContext.Entry(paymentVoucher).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    dB_LogisticsproContext.LpFPaymentVoucherLineItem.Remove(lpFPaymentVoucherLineItem);
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<LineGenerateItem> GetJobCardItemsByCustomerId(int customerId, DateTime? fromDate, DateTime? toDate, string? jobCardNumber)
        {
            try
            {
                LineGenerateItem lineGenerateItem = new LineGenerateItem();

                var batchTransportItems = await (from j in dB_LogisticsproContext.LpJobCard
                                        join ljt in dB_LogisticsproContext.LpJTransportation on j.Id equals ljt.JobCardId
                                        join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                        where ljt.IsInvoiced == false &&
                                        j.CustomerId == customerId &&
                                        (fromDate == null || (ljt.PickupTime.HasValue && ljt.PickupTime.Value.Date >= fromDate.Value.Date)) &&
                                        (toDate == null || (ljt.PickupTime.HasValue && ljt.PickupTime.Value.Date <= toDate.Value.Date)) &&
                                        (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                 select new JobCardTransportation
                                        {
                                            Id = ljt.Id,
                                            JobCardNo = j.JobCardCode,
                                            JobCardDescription = j.JobDescription,
                                            JobCardId = ljt.JobCardId,
                                            CustomerName = c.CustomerName,
                                            BookingRef = ljt.TransportationCode,
                                            CustomerRef = ljt.CustomerRef,
                                            Remarks = ljt.Remarks,
                                            DropoffLocation = ljt.DropoffLocation,
                                            CostBaseAmount = ljt.CostBaseAmount,
                                            CostTaxAmount = ljt.CostTaxAmount,
                                            IsVatIncludedSell = ljt.IsVatIncludedSell,
                                            IsVatIncludedCost = ljt.IsVatIncludedCost,
                                            SellBaseAmount = ljt.SellBaseAmount,
                                            SellTaxAmount = ljt.SellTaxAmount,
                                            PickupLocation = ljt.PickupLocation,
                                            PickupTime = ljt.PickupTime,
                                            Adults = ljt.Adults,
                                            Children = ljt.Children,
                                            Infants = ljt.Infants,
                                            VehicleType = ljt.VehicleType,
                                            FlightNo = ljt.FlightNo,
                                            FlightTime = ljt.FlightTime,
                                            PaxName = ljt.PaxName,
                                            Parking = ljt.Parking,
                                            ParkingTaxAmount= ljt.ParkingTaxAmount,
                                            Extras = ljt.Extras,
                                            ExtrasTaxAmount = ljt.ExtrasTaxAmount,
                                            Water = ljt.Water,
                                            WaterTaxAmount= ljt.WaterTaxAmount,
                                            ParkingSell = ljt.ParkingSell,
                                            ParkingTaxAmountSell = ljt.ParkingTaxAmountSell,
                                            ExtrasSell = ljt.ExtrasSell,
                                            ExtrasTaxAmountSell = ljt.ExtrasTaxAmountSell,
                                            WaterSell = ljt.WaterSell,
                                            WaterTaxAmountSell = ljt.WaterTaxAmountSell
                                        }).ToListAsync();
                if (batchTransportItems != null && batchTransportItems.Any())
                {
                    lineGenerateItem.Transportations.AddRange(batchTransportItems);
                }

                var batchHotelItems = await (from j in dB_LogisticsproContext.LpJobCard
                                                 join ljh in dB_LogisticsproContext.LpJHotel on j.Id equals ljh.JobCardId
                                                 join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                 where ljh.IsInvoiced == false && 
                                                 ljh.VendorId.HasValue && ljh.VendorId>0 &&
                                                 j.CustomerId == customerId &&
                                                 (fromDate == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date >= fromDate.Value.Date)) &&
                                                 (toDate == null || (ljh.CheckIn.HasValue && ljh.CheckIn.Value.Date <= toDate.Value.Date)) &&
                                        (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                             select new JobCardHotel
                                                 {
                                                     Id = ljh.Id,
                                                     HotelCode = ljh.HotelCode,
                                                     PaxName = ljh.PaxName,
                                                     CheckIn = ljh.CheckIn,
                                                     CheckOut = ljh.CheckOut,
                                                     Adults = ljh.Adults,
                                                     Children = ljh.Children,
                                                     Infants = ljh.Infants,
                                                     HotelName = ljh.HotelName,
                                                     CostBaseAmount = ljh.CostBaseAmount,
                                                     CostTaxAmount = ljh.CostTaxAmount,
                                                     SellBaseAmount = ljh.SellBaseAmount,
                                                     SellTaxAmount = ljh.SellTaxAmount,
                                                     Remarks = ljh.Remarks,
                                                     RoomType = ljh.RoomType,
                                                     HotelConfirmation = ljh.HotelConfirmation
                                                 }).ToListAsync();

                if (batchHotelItems != null && batchHotelItems.Any())
                {
                    lineGenerateItem.Hotels.AddRange(batchHotelItems);
                }

                var batchVisaItems = await (from j in dB_LogisticsproContext.LpJobCard
                                             join ljv in dB_LogisticsproContext.LpJVisa on j.Id equals ljv.JobCardId
                                            join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                            join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                             where ljv.IsInvoiced == false && 
                                             j.CustomerId == customerId &&
                                             ljv.VendorId.HasValue && ljv.VendorId > 0 &&
                                             (fromDate == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date >= fromDate.Value.Date)) &&
                                             (toDate == null || (ljv.CreatedDate.HasValue && ljv.CreatedDate.Value.Date <= toDate.Value.Date)) &&
                                        (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                            select new JobCardVisa
                                             {
                                                 Id = ljv.Id,
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
                                                 Remarks = ljv.Remarks
                                             }).ToListAsync();

                if (batchVisaItems != null && batchVisaItems.Any())
                {
                    lineGenerateItem.Visa.AddRange(batchVisaItems);
                }


                var batchMiscellaneousItems = await (from j in dB_LogisticsproContext.LpJobCard
                                            join ljm in dB_LogisticsproContext.LpJMiscellaneous on j.Id equals ljm.JobCardId
                                            join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                            where ljm.IsInvoiced == false && 
                                            j.CustomerId == customerId &&
                                            ljm.VendorId.HasValue && ljm.VendorId > 0 &&
                                            (fromDate == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date >= fromDate.Value.Date)) &&
                                            (toDate == null || (ljm.MisDate.HasValue && ljm.MisDate.Value.Date <= toDate.Value.Date)) &&
                                        (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                     select new JobCardMiscellaneous
                                            {
                                                Id = ljm.Id,
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
                                                Remarks = ljm.Remarks
                                            }).ToListAsync();

                if (batchMiscellaneousItems != null && batchMiscellaneousItems.Any())
                {
                    lineGenerateItem.Miscellaneous.AddRange(batchMiscellaneousItems);
                }

                return lineGenerateItem;
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<List<ProformaInvoice>> GetProformaInvoiceReceiptsByJobCardIds(List<int?> jobCardIds)
        {
            try
            {
                List<ProformaInvoice> proformaInvoices = new List<ProformaInvoice>();

                foreach (var jobCardId in jobCardIds)
                {
                    var invoices = await (from i in dB_LogisticsproContext.LpFProformaInvoice
                                          join j in dB_LogisticsproContext.LpJobCard on i.JobCardId equals j.Id
                                          where i.StatusId != 3 && i.JobCardId== jobCardId
                                          select new ProformaInvoice
                                          {
                                              Id = i.Id,
                                              InvoiceCode=i.InvoiceCode,
                                              InvoiceDate=i.InvoiceDate,
                                              InvoiceAmount=i.InvoiceAmount,
                                              JobCardId = j.Id,
                                              JobCardNo = j.JobCardCode
                                          }).ToListAsync();

                    foreach (var invoice in invoices)
                    {
                        var filterProformaInvoice=await dB_LogisticsproContext.LpFInvoiceProformaInvoice.Where(i=>i.ProformaInvoiceId==invoice.Id && i.JobCardId== invoice.JobCardId).ToListAsync();
                        if(filterProformaInvoice.Any())
                        {
                            continue;
                        }
                        var invoiceReceipts = await (from ir in dB_LogisticsproContext.LpFProformaInvoiceReceipt
                                                     join j in dB_LogisticsproContext.LpJobCard on ir.JobCardId equals j.Id
                                                     where ir.IsAllocatedToInvoice == false && ir.JobCardId== jobCardId && ir.ProformaInvoiceId== invoice.Id
                                                     select new ProformaInvoiceReceipt
                                                     {
                                                         PaymentDate = ir.PaymentDate,
                                                         PaymentMethodId = ir.PaymentMethodId,
                                                         Remark = ir.Remark,
                                                         Amount = ir.Amount,
                                                     }).ToListAsync();
                        invoice.ProformaInvoiceReceipts= invoiceReceipts;
                        proformaInvoices.AddRange(invoices);
                    }
                }

                return proformaInvoices.Distinct().OrderBy(i => i.InvoiceDate).ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<ProformaInvoice>> GetLinkedProformaInvoiceReceipts(int invoiceId)
        {
            try
            {
                List<ProformaInvoice> proformaInvoices = new List<ProformaInvoice>();

                var linkedInvoiceReceipts=await dB_LogisticsproContext.LpFInvoiceProformaInvoice.Where(i=>i.InvoiceId== invoiceId).ToListAsync();

                foreach (var linkedInvoiceReceipt in linkedInvoiceReceipts)
                {
                    var invoices = await (from i in dB_LogisticsproContext.LpFProformaInvoice
                                          join j in dB_LogisticsproContext.LpJobCard on i.JobCardId equals j.Id
                                          where i.StatusId != 3 && i.JobCardId == linkedInvoiceReceipt.JobCardId && i.Id== linkedInvoiceReceipt.ProformaInvoiceId
                                          select new ProformaInvoice
                                          {
                                              Id = i.Id,
                                              InvoiceCode = i.InvoiceCode,
                                              InvoiceDate = i.InvoiceDate,
                                              InvoiceAmount = i.InvoiceAmount,
                                              JobCardId = j.Id,
                                              JobCardNo = j.JobCardCode
                                          }).ToListAsync();

                    foreach (var invoice in invoices)
                    {
                        var invoiceReceipts = await (from ir in dB_LogisticsproContext.LpFProformaInvoiceReceipt
                                                     join j in dB_LogisticsproContext.LpJobCard on ir.JobCardId equals j.Id
                                                     where ir.IsAllocatedToInvoice == false && ir.JobCardId == invoice.JobCardId && ir.ProformaInvoiceId == invoice.Id
                                                     select new ProformaInvoiceReceipt
                                                     {
                                                         PaymentDate = ir.PaymentDate,
                                                         PaymentMethodId = ir.PaymentMethodId,
                                                         Remark = ir.Remark,
                                                         Amount = ir.Amount,
                                                     }).ToListAsync();
                        invoice.ProformaInvoiceReceipts = invoiceReceipts;
                        proformaInvoices.AddRange(invoices);
                    }
                }

                return proformaInvoices.Distinct().OrderBy(i => i.InvoiceDate).ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> SaveInvoice(InvoiceSaveRequest invoiceSaveRequest)
        {
            try
            {
                var invoicePrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "InvoicePrefix");
                var currentInvoiceNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentInvoiceNumber");
                string invoicePrefixCode = string.Empty;
                int nextNumber = 0;

                if (invoicePrefix != null && currentInvoiceNumber != null)
                {
                    nextNumber = int.Parse(currentInvoiceNumber.SystemValue) + 1;
                    invoicePrefixCode = $"{invoicePrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpFInvoice lpFInvoice = new LpFInvoice();
                lpFInvoice.InvoiceCode = invoicePrefixCode;
                lpFInvoice.CustomerId = invoiceSaveRequest.CustomerId;
                lpFInvoice.InvoiceAmount = invoiceSaveRequest.InvoiceAmount;
                lpFInvoice.InvoiceDate = invoiceSaveRequest.InvoiceDate;
                lpFInvoice.InvoiceDueDate = invoiceSaveRequest.InvoiceDueDate;
                lpFInvoice.Remark = invoiceSaveRequest.Remarks;
                lpFInvoice.TransportDescription = invoiceSaveRequest.TransportDescription;
                lpFInvoice.HotelDescription = invoiceSaveRequest.HotelDescription;
                lpFInvoice.VisaDescription = invoiceSaveRequest.VisaDescription;
                lpFInvoice.MiscellaneousDescription = invoiceSaveRequest.MiscellaneousDescription;
                lpFInvoice.StatusId = (int)EnumInvoiceStatus.Generated;
                lpFInvoice.CreatedBy = invoiceSaveRequest.UserId;
                lpFInvoice.CreatedDate = DateTime.UtcNow;
                lpFInvoice.UpdatedBy = invoiceSaveRequest.UserId;
                lpFInvoice.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpFInvoice.Add(lpFInvoice);
                await dB_LogisticsproContext.SaveChangesAsync();

                foreach (var performaInvoiceId in invoiceSaveRequest.PerformaInvoiceIds)
                {
                    var proformaInvoice = await dB_LogisticsproContext.LpFProformaInvoice.FirstOrDefaultAsync(i => i.Id == performaInvoiceId);
                    if (proformaInvoice != null)
                    {
                        LpFInvoiceProformaInvoice lpFInvoiceProformaInvoice = new LpFInvoiceProformaInvoice();
                        lpFInvoiceProformaInvoice.InvoiceId = lpFInvoice.Id;
                        lpFInvoiceProformaInvoice.ProformaInvoiceId = performaInvoiceId;
                        lpFInvoiceProformaInvoice.JobCardId = proformaInvoice.JobCardId;
                        lpFInvoiceProformaInvoice.CreatedBy = invoiceSaveRequest.UserId;
                        lpFInvoiceProformaInvoice.CreatedDate = DateTime.UtcNow;
                        lpFInvoiceProformaInvoice.UpdatedBy = invoiceSaveRequest.UserId;
                        lpFInvoiceProformaInvoice.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFInvoiceProformaInvoice.Add(lpFInvoiceProformaInvoice);
                        await dB_LogisticsproContext.SaveChangesAsync();



                        proformaInvoice.IsAllocatedToInvoice = true;
                        proformaInvoice.UpdatedBy = invoiceSaveRequest.UserId;
                        proformaInvoice.UpdatedDate = DateTime.UtcNow;
                        dB_LogisticsproContext.Entry(proformaInvoice).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }
                }
                

                currentInvoiceNumber.SystemValue = nextNumber.ToString();
                currentInvoiceNumber.UpdatedDate = DateTime.UtcNow;
                currentInvoiceNumber.UpdatedBy = invoiceSaveRequest.UserId;
                dB_LogisticsproContext.Entry(currentInvoiceNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                foreach (var invoiceItemId in invoiceSaveRequest.TransportationIds)
                {
                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(o => o.Id == invoiceItemId);
                    if (lpJTransportation != null)
                    {
                        LpFInvoiceLineItem invoiceLineItem = new LpFInvoiceLineItem();
                        invoiceLineItem.InvoiceId = lpFInvoice.Id;
                        invoiceLineItem.SellBaseAmount = lpJTransportation.SellBaseAmount;
                        invoiceLineItem.SellTaxAmount= lpJTransportation.SellTaxAmount;
                        invoiceLineItem.ContextId = invoiceItemId;
                        invoiceLineItem.ContextTypeId = (int)EnumContextType.Transpotation;
                        invoiceLineItem.CreatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.CreatedDate = DateTime.UtcNow;
                        invoiceLineItem.UpdatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFInvoiceLineItem.Add(invoiceLineItem);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        lpJTransportation.IsInvoiced = true;
                        lpJTransportation.UpdatedBy = invoiceSaveRequest.UserId;
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
                    }

                }

                foreach (var invoiceItemId in invoiceSaveRequest.HotelIds)
                {
                    var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(o => o.Id == invoiceItemId);
                    if (lpJHotel != null)
                    {
                        LpFInvoiceLineItem invoiceLineItem = new LpFInvoiceLineItem();
                        invoiceLineItem.InvoiceId = lpFInvoice.Id;
                        invoiceLineItem.SellBaseAmount = lpJHotel.SellBaseAmount;
                        invoiceLineItem.SellTaxAmount = lpJHotel.SellTaxAmount;
                        invoiceLineItem.ContextId = invoiceItemId;
                        invoiceLineItem.ContextTypeId = (int)EnumContextType.Hotel;
                        invoiceLineItem.CreatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.CreatedDate = DateTime.UtcNow;
                        invoiceLineItem.UpdatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFInvoiceLineItem.Add(invoiceLineItem);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        lpJHotel.IsInvoiced = true;
                        lpJHotel.UpdatedBy = invoiceSaveRequest.UserId;
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
                    }

                }

                foreach (var invoiceItemId in invoiceSaveRequest.VisaIds)
                {
                    var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(o => o.Id == invoiceItemId);
                    if (lpJVisa != null)
                    {
                        LpFInvoiceLineItem invoiceLineItem = new LpFInvoiceLineItem();
                        invoiceLineItem.InvoiceId = lpFInvoice.Id;
                        invoiceLineItem.SellBaseAmount = lpJVisa.SellBaseAmount;
                        invoiceLineItem.SellTaxAmount = lpJVisa.SellTaxAmount;
                        invoiceLineItem.ContextId = invoiceItemId;
                        invoiceLineItem.ContextTypeId = (int)EnumContextType.Visa;
                        invoiceLineItem.CreatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.CreatedDate = DateTime.UtcNow;
                        invoiceLineItem.UpdatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFInvoiceLineItem.Add(invoiceLineItem);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        lpJVisa.IsInvoiced = true;
                        lpJVisa.UpdatedBy = invoiceSaveRequest.UserId;
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
                    }

                }

                foreach (var invoiceItemId in invoiceSaveRequest.MiscellaneousIds)
                {
                    var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(o => o.Id == invoiceItemId);
                    if (lpJMiscellaneous != null)
                    {
                        LpFInvoiceLineItem invoiceLineItem = new LpFInvoiceLineItem();
                        invoiceLineItem.InvoiceId = lpFInvoice.Id;
                        invoiceLineItem.SellBaseAmount = lpJMiscellaneous.SellBaseAmount;
                        invoiceLineItem.SellTaxAmount = lpJMiscellaneous.SellTaxAmount;
                        invoiceLineItem.ContextId = invoiceItemId;
                        invoiceLineItem.ContextTypeId = (int)EnumContextType.Miscellaneous;
                        invoiceLineItem.CreatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.CreatedDate = DateTime.UtcNow;
                        invoiceLineItem.UpdatedBy = invoiceSaveRequest.UserId;
                        invoiceLineItem.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFInvoiceLineItem.Add(invoiceLineItem);
                        await dB_LogisticsproContext.SaveChangesAsync();

                        lpJMiscellaneous.IsInvoiced = true;
                        lpJMiscellaneous.UpdatedBy = invoiceSaveRequest.UserId;
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
                    }

                }
                return true;
            }
            catch (Exception e)
            {

                throw;
                return false;
            }
            return false;

        }

        public async Task<List<Invoice>> GetInvoices(bool isFirstLoad, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId,string? jobCardNumber)
        {
            try
            {
                int takeValue = 100000;
                if (isFirstLoad)
                {
                    takeValue = 10;
                }
                var countries = await GetCountries();

                var invoiceList = new List<Invoice>();
                var lpFInvoice = await (from i in dB_LogisticsproContext.LpFInvoice
                                                join lpc in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals lpc.Id
                                                join s in dB_LogisticsproContext.LpMInvoiceStatus on i.StatusId equals s.Id
                                                //join c in countries on lpc.CountryCode equals c.Ccn3
                                                where (invoiceCode == null || i.InvoiceCode.Contains(invoiceCode)) &&
                                                (invoiceDateFrom == null || i.InvoiceDate >= invoiceDateFrom) &&
                                                (invoiceDateTo == null || i.InvoiceDate <= invoiceDateTo) &&
                                                (invoiceDueDateFrom == null || i.InvoiceDueDate >= invoiceDueDateFrom) &&
                                                (invoiceDueDateTo == null || i.InvoiceDueDate <= invoiceDueDateTo) 
                                                select new Invoice
                                                {
                                                    Id = i.Id,
                                                    InvoiceCode = i.InvoiceCode,
                                                    CustomerId = i.CustomerId,
                                                    CustomerName = lpc.CustomerName,
                                                    CustomerAddressLine1 = lpc.Address1 == null || lpc.Address1 == "" ? lpc.Address2 : $"{lpc.Address1}, {lpc.Address2} ",
                                                    //CustomerAddressLine2 = lpc.City == null || lpc.City == "" ? c.Name.Common : $"{lpc.City}, {c.Name.Common} ",
                                                    City = lpc.City,
                                                    CustomerTrn = lpc.Trn,
                                                    CountryCode = lpc.CountryCode,
                                                    InvoiceDate = i.InvoiceDate,
                                                    InvoiceDueDate = i.InvoiceDueDate,
                                                    InvoiceAmount = i.InvoiceAmount,
                                                    TransportDescription = i.TransportDescription,
                                                    HotelDescription = i.HotelDescription,
                                                    VisaDescription = i.VisaDescription,
                                                    MiscellaneousDescription = i.MiscellaneousDescription,
                                                    StatusName = s.StatusName,
                                                    StatusId = s.Id
                                                }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync();

                var filterInvoiceList = (from i in lpFInvoice   
                                         where (customerId == null || customerId.Count == 0 || customerId.Any(x => i.CustomerId == x)) &&
                                         (statusId == null || statusId.Count == 0 || statusId.Any(x => x==0|| i.StatusId == x))
                                         select i).OrderByDescending(i => i.Id).ToList();

                invoiceList.AddRange(filterInvoiceList);

                foreach (var invoice in invoiceList)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoice.CountryCode);
                    var countryName= country!=null ? country.Name : "";

                    invoice.CustomerAddressLine2 = invoice.City == null || invoice.City == "" ? countryName : $"{invoice.City}, {countryName} ";

                    var lineItems = new LineGenerateItem();

                    var transpostBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                     join ljt in dB_LogisticsproContext.LpJTransportation on il.ContextId equals ljt.Id
                                                     join j in dB_LogisticsproContext.LpJobCard on ljt.JobCardId equals j.Id
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     where il.InvoiceId == invoice.Id && il.ContextTypeId==(int)EnumContextType.Transpotation &&
                                                     (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                     orderby ljt.PickupTime
                                                     select new JobCardTransportation
                                                     {
                                                         Id = il.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         JobCardDescription = j.JobDescription,
                                                         JobCardId = ljt.JobCardId,
                                                         CustomerName = c.CustomerName,
                                                         BookingRef = ljt.TransportationCode,
                                                         CustomerRef = ljt.CustomerRef,
                                                         Remarks = ljt.Remarks,
                                                         DropoffLocation = ljt.DropoffLocation,
                                                         CostBaseAmount = ljt.CostBaseAmount,
                                                         CostTaxAmount = ljt.CostTaxAmount,
                                                         IsVatIncludedSell = ljt.IsVatIncludedSell,
                                                         IsVatIncludedCost = ljt.IsVatIncludedCost,
                                                         SellBaseAmount = il.SellBaseAmount,
                                                         SellTaxAmount = il.SellTaxAmount,
                                                         PickupLocation = ljt.PickupLocation,
                                                         PickupTime = ljt.PickupTime,
                                                         Adults = ljt.Adults,
                                                         Children = ljt.Children,
                                                         Infants = ljt.Infants,
                                                         VehicleType = ljt.VehicleType,
                                                         FlightNo = ljt.FlightNo,
                                                         FlightTime = ljt.FlightTime,
                                                         PaxName = ljt.PaxName,
                                                         Parking = ljt.Parking,
                                                         ParkingTaxAmount = ljt.ParkingTaxAmount,
                                                         Extras = ljt.Extras,
                                                         ExtrasTaxAmount = ljt.ExtrasTaxAmount,
                                                         Water = ljt.Water,
                                                         WaterTaxAmount = ljt.WaterTaxAmount,
                                                         ParkingSell = ljt.ParkingSell,
                                                         ParkingTaxAmountSell = ljt.ParkingTaxAmountSell,
                                                         ExtrasSell = ljt.ExtrasSell,
                                                         ExtrasTaxAmountSell = ljt.ExtrasTaxAmountSell,
                                                         WaterSell = ljt.WaterSell,
                                                         WaterTaxAmountSell = ljt.WaterTaxAmountSell
                                                     }
                                                     ).Distinct().ToListAsync();

                    lineItems.Transportations.AddRange(transpostBatchItems);

                    var hotelBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                     join ljh in dB_LogisticsproContext.LpJHotel on il.ContextId equals ljh.Id
                                                     join j in dB_LogisticsproContext.LpJobCard on ljh.JobCardId equals j.Id
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Hotel &&
                                                     (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                 orderby ljh.CheckIn
                                                     select new JobCardHotel
                                                     {
                                                         Id = il.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         HotelCode = ljh.HotelCode,
                                                         PaxName = ljh.PaxName,
                                                         CheckIn = ljh.CheckIn,
                                                         CheckOut = ljh.CheckOut,
                                                         Adults = ljh.Adults,
                                                         Children = ljh.Children,
                                                         Infants = ljh.Infants,
                                                         HotelName = ljh.HotelName,
                                                         CostBaseAmount = ljh.CostBaseAmount,
                                                         CostTaxAmount = ljh.CostTaxAmount,
                                                         IsVatIncludedSell = ljh.IsVatIncludedSell,
                                                         IsVatIncludedCost = ljh.IsVatIncludedCost,
                                                         SellBaseAmount = il.SellBaseAmount,
                                                         SellTaxAmount = il.SellTaxAmount,
                                                         Remarks = ljh.Remarks,
                                                         RoomType = ljh.RoomType,
                                                         HotelConfirmation = ljh.HotelConfirmation
                                                     }).Distinct().ToListAsync();

                    lineItems.Hotels.AddRange(hotelBatchItems);

                    var visaBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                 join ljv in dB_LogisticsproContext.LpJVisa on il.ContextId equals ljv.Id
                                                join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                                join j in dB_LogisticsproContext.LpJobCard on ljv.JobCardId equals j.Id
                                                 join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                 where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Visa &&
                                                 (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                orderby ljv.CreatedDate
                                                 select new JobCardVisa
                                                 {
                                                     Id = il.Id,
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
                                                     Remarks = ljv.Remarks
                                                 }).Distinct().ToListAsync();

                    lineItems.Visa.AddRange(visaBatchItems);

                    var miscellaneousBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                 join ljm in dB_LogisticsproContext.LpJMiscellaneous on il.ContextId equals ljm.Id
                                                 join j in dB_LogisticsproContext.LpJobCard on ljm.JobCardId equals j.Id
                                                 join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                 where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Miscellaneous &&
                                                 (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                         orderby ljm.MisDate
                                                 select new JobCardMiscellaneous
                                                 {
                                                     Id = il.Id,
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
                                                     Remarks = ljm.Remarks
                                                 }).Distinct().ToListAsync();

                    lineItems.Miscellaneous.AddRange(miscellaneousBatchItems);

                    invoice.InvoiceLineItems = lineItems;


                }
                return invoiceList.Where(i => i.InvoiceLineItems.Transportations.Any() || i.InvoiceLineItems.Hotels.Any() || i.InvoiceLineItems.Visa.Any() || i.InvoiceLineItems.Miscellaneous.Any()).ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        //public async Task<List<Country>> GetCountries()
        //{
        //    using var client = new HttpClient();
        //    string apiUri = $"https://restcountries.com/v3.1/all";
        //    HttpResponseMessage response = client.GetAsync(apiUri).Result;

        //    var countries = JsonConvert.DeserializeObject<List<Country>>(response.Content.ReadAsStringAsync().Result);

        //    return countries.ToList();
        //}

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

        public async Task<bool> VoidInvoice(int id,int userId)
        {
            var invoice = await dB_LogisticsproContext.LpFInvoice.FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return false;
            }

            var invoiceProformaInvoices = await dB_LogisticsproContext.LpFInvoiceProformaInvoice.Where(i=>i.InvoiceId== invoice.Id).ToListAsync();

            foreach (var invoiceProformaInvoice in invoiceProformaInvoices)
            {
                var proformaInvoice = await dB_LogisticsproContext.LpFProformaInvoice.FirstOrDefaultAsync(i => i.Id == invoiceProformaInvoice.ProformaInvoiceId);
                if (proformaInvoice != null)
                {
                    proformaInvoice.IsAllocatedToInvoice = false;
                    proformaInvoice.UpdatedBy = userId;
                    proformaInvoice.UpdatedDate = DateTime.UtcNow;
                    dB_LogisticsproContext.Entry(proformaInvoice).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }
            

            var invoiceItems=await dB_LogisticsproContext.LpFInvoiceLineItem.Where(i=>i.InvoiceId== invoice.Id).ToListAsync();

            foreach (var invoiceItem in invoiceItems)
            {
                if (invoiceItem.ContextTypeId == (int)EnumContextType.Transpotation)
                {
                    var lpJTransportation = await dB_LogisticsproContext.LpJTransportation.FirstOrDefaultAsync(i => i.Id == invoiceItem.ContextId);
                    if (lpJTransportation != null)
                    {
                        lpJTransportation.IsInvoiced = false;
                        lpJTransportation.UpdatedDate = DateTime.UtcNow;
                        lpJTransportation.UpdatedBy = userId;
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
                    }
                }

                if (invoiceItem.ContextTypeId == (int)EnumContextType.Hotel)
                {
                    var lpJHotel = await dB_LogisticsproContext.LpJHotel.FirstOrDefaultAsync(i => i.Id == invoiceItem.ContextId);
                    if (lpJHotel != null)
                    {
                        lpJHotel.IsInvoiced = false;
                        lpJHotel.UpdatedDate = DateTime.UtcNow;
                        lpJHotel.UpdatedBy = userId;
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
                    }
                }

                if (invoiceItem.ContextTypeId == (int)EnumContextType.Visa)
                {
                    var lpJVisa = await dB_LogisticsproContext.LpJVisa.FirstOrDefaultAsync(i => i.Id == invoiceItem.ContextId);
                    if (lpJVisa != null)
                    {
                        lpJVisa.IsInvoiced = false;
                        lpJVisa.UpdatedDate = DateTime.UtcNow;
                        lpJVisa.UpdatedBy = userId;
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
                    }
                }

                if (invoiceItem.ContextTypeId == (int)EnumContextType.Miscellaneous)
                {
                    var lpJMiscellaneous = await dB_LogisticsproContext.LpJMiscellaneous.FirstOrDefaultAsync(i => i.Id == invoiceItem.ContextId);
                    if (lpJMiscellaneous != null)
                    {
                        lpJMiscellaneous.IsInvoiced = false;
                        lpJMiscellaneous.UpdatedDate = DateTime.UtcNow;
                        lpJMiscellaneous.UpdatedBy = userId;
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
                    }
                }
            }

             invoice.StatusId = 3;
            dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
            await dB_LogisticsproContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveProformaInvoice(ProformaInvoiceSaveRequest proformaInvoiceSaveRequest)
        {
            try
            {
                var proformaInvoicePrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "ProformaInvoicePrefix");
                var currentProformaInvoiceNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentProformaInvoiceNumber");
                string proformaInvoicePrefixCode = string.Empty;
                int nextNumber = 0;

                if (proformaInvoicePrefix != null && currentProformaInvoiceNumber != null)
                {
                    nextNumber = int.Parse(currentProformaInvoiceNumber.SystemValue) + 1;
                    proformaInvoicePrefixCode = $"{proformaInvoicePrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpFProformaInvoice lpFProformaInvoice = new LpFProformaInvoice();
                lpFProformaInvoice.InvoiceCode = proformaInvoicePrefixCode;
                lpFProformaInvoice.CustomerId = proformaInvoiceSaveRequest.CustomerId;
                lpFProformaInvoice.InvoiceAmount = proformaInvoiceSaveRequest.InvoiceAmount;
                lpFProformaInvoice.InvoiceDate = proformaInvoiceSaveRequest.InvoiceDate;
                lpFProformaInvoice.InvoiceDueDate = proformaInvoiceSaveRequest.InvoiceDueDate;
                lpFProformaInvoice.JobCardId = proformaInvoiceSaveRequest.JobCardId;
                lpFProformaInvoice.Description = proformaInvoiceSaveRequest.Description;
                lpFProformaInvoice.StatusId = (int)EnumInvoiceStatus.Generated;
                lpFProformaInvoice.CreatedBy = proformaInvoiceSaveRequest.UserId;
                lpFProformaInvoice.CreatedDate = DateTime.UtcNow;
                lpFProformaInvoice.UpdatedBy = proformaInvoiceSaveRequest.UserId;
                lpFProformaInvoice.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpFProformaInvoice.Add(lpFProformaInvoice);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentProformaInvoiceNumber.SystemValue = nextNumber.ToString();
                currentProformaInvoiceNumber.UpdatedDate = DateTime.UtcNow;
                currentProformaInvoiceNumber.UpdatedBy= proformaInvoiceSaveRequest.UserId;
                dB_LogisticsproContext.Entry(currentProformaInvoiceNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
                return false;
            }
            return false;

        }

        public async Task<bool> VoidProformaInvoice(int id, int userId)
        {
            var invoice = await dB_LogisticsproContext.LpFProformaInvoice.FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return false;
            }

            invoice.StatusId = 3;
            invoice.UpdatedBy= userId;
            invoice.UpdatedDate = DateTime.UtcNow;
            dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
            await dB_LogisticsproContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<ProformaInvoice>> GetProformaInvoices(bool isFirstLoad, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId, string? jobCardNumber)
        {
            try
            {
                int takeValue = 100000;
                if (isFirstLoad)
                {
                    takeValue = 10;
                }
                var countries = await GetCountries();

                var invoiceList = new List<ProformaInvoice>();
                var lpFProformaInvoice = await (from i in dB_LogisticsproContext.LpFProformaInvoice
                                                join lpc in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals lpc.Id
                                                join s in dB_LogisticsproContext.LpMInvoiceStatus on i.StatusId equals s.Id
                                                join j in dB_LogisticsproContext.LpJobCard on i.JobCardId equals j.Id
                                                //join c in countries on lpc.CountryCode equals c.Ccn3
                                                where (invoiceCode == null || i.InvoiceCode.Contains(invoiceCode)) &&
                                                (invoiceDateFrom == null || i.InvoiceDate >= invoiceDateFrom) &&
                                                (invoiceDateTo == null || i.InvoiceDate <= invoiceDateTo) &&
                                                (invoiceDueDateFrom == null || i.InvoiceDueDate >= invoiceDueDateFrom) &&
                                                (invoiceDueDateTo == null || i.InvoiceDueDate <= invoiceDueDateTo) &&
                                                (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                                select new ProformaInvoice
                                                {
                                                    Id = i.Id,
                                                    InvoiceCode = i.InvoiceCode,
                                                    CustomerId = i.CustomerId,
                                                    CustomerName = lpc.CustomerName,
                                                    CustomerAddressLine1 = lpc.Address1 == null || lpc.Address1 == "" ? lpc.Address2 : $"{lpc.Address1}, {lpc.Address2} ",
                                                    //CustomerAddressLine2 = lpc.City == null || lpc.City == "" ? c.Name.Common : $"{lpc.City}, {c.Name.Common} ",
                                                    City = lpc.City,
                                                    CountryCode = lpc.CountryCode,
                                                    InvoiceDate = i.InvoiceDate,
                                                    InvoiceDueDate = i.InvoiceDueDate,
                                                    InvoiceAmount = i.InvoiceAmount,
                                                    Description = i.Description,
                                                    StatusName = s.StatusName,
                                                    JobCardId = j.Id,
                                                    JobCardNo = j.JobCardCode,
                                                    StatusId = s.Id
                                                }).OrderByDescending(i => i.Id).Take(takeValue).ToListAsync();

                //foreach (var cId in customerId)
                //{
                //    filterInvoiceList.AddRange(lpFProformaInvoice.Where(i => i.CustomerId == cId).ToList());
                //}

                var filterInvoiceList = (from i in lpFProformaInvoice
                                         where (customerId == null || customerId.Count == 0 || customerId.Any(x => i.CustomerId == x)) &&
                                         (statusId == null || statusId.Count == 0 || statusId.Any(x => x==0||i.StatusId == x))
                                         select i).OrderByDescending(i => i.Id).ToList();

                invoiceList.AddRange(filterInvoiceList);

                foreach (var invoice in invoiceList)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoice.CountryCode);
                    var countryName = country != null ? country.Name : "";

                    invoice.CustomerAddressLine2 = invoice.City == null || invoice.City == "" ? countryName : $"{invoice.City}, {countryName} ";
                }
                return invoiceList.ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<ProformaInvoice> GetProformaInvoice(int id)
        {
            try
            {
                var countries = await GetCountries();
                var invoice = await (from i in dB_LogisticsproContext.LpFProformaInvoice
                                     join lpc in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals lpc.Id
                                     join s in dB_LogisticsproContext.LpMInvoiceStatus on i.StatusId equals s.Id
                                     join j in dB_LogisticsproContext.LpJobCard on i.JobCardId equals j.Id
                                     //join c in countries on lpc.CountryCode equals c.Ccn3
                                     where (i.Id == id)
                                     select new ProformaInvoice
                                     {
                                         Id = i.Id,
                                         InvoiceCode = i.InvoiceCode,
                                         CustomerId = i.CustomerId,
                                         CustomerName = lpc.CustomerName,
                                         CustomerAddressLine1 = lpc.Address1 == null || lpc.Address1 == "" ? lpc.Address2 : $"{lpc.Address1}, {lpc.Address2} ",
                                         City = lpc.City,
                                         CountryCode = lpc.CountryCode,
                                         InvoiceDate = i.InvoiceDate,
                                         InvoiceDueDate = i.InvoiceDueDate,
                                         InvoiceAmount = i.InvoiceAmount,
                                         Description = i.Description,
                                         StatusName = s.StatusName,
                                         JobCardId = j.Id,
                                         JobCardNo = j.JobCardCode,
                                         StatusId = s.Id
                                     }).OrderByDescending(i => i.Id).FirstOrDefaultAsync();

                if (invoice != null)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoice.CountryCode);
                    var countryName = country != null ? country.Name : "";

                    invoice.CustomerAddressLine2 = invoice.City == null || invoice.City == "" ? countryName : $"{invoice.City}, {countryName} ";
                }
                return invoice == null ? new ProformaInvoice() : invoice;
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<List<ProformaInvoiceReceipt>> GetProformaInvoiceReceipts(int id)
        {
            try
            {
                var countries = await GetCountries();
                var invoiceReceipts = await (from ir in dB_LogisticsproContext.LpFProformaInvoiceReceipt
                                     join i in dB_LogisticsproContext.LpFProformaInvoice on ir.ProformaInvoiceId equals i.Id
                                     join c in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals c.Id
                                     join pm in dB_LogisticsproContext.LpMPaymentMethod on ir.PaymentMethodId equals pm.Id
                                     join j in dB_LogisticsproContext.LpJobCard on ir.JobCardId equals j.Id
                                     where (ir.ProformaInvoiceId == id)
                                     select new ProformaInvoiceReceipt
                                     {
                                         Id = ir.Id,
                                         JobCardId=ir.JobCardId,
                                         PaymentMethod=pm.PaymentMethodName,
                                         ProformaInvoiceId=i.Id,
                                         PaymentDate=ir.PaymentDate,
                                         PaymentMethodId=ir.PaymentMethodId,
                                         Remark=ir.Remark,
                                         JobCardNo = j.JobCardCode,
                                         Amount=ir.Amount,
                                         ProformaInvoiceCode=i.InvoiceCode,
                                         ReceiptCode=ir.ReceiptCode,
                                         CustomerName=c.CustomerName,
                                         CountryCode=c.CountryCode,
                                         City=c.City,
                                         AddressLine1= c.Address1 == null || c.Address1 == "" ? c.Address2 : $"{c.Address1}, {c.Address2} ",
                                     }).OrderByDescending(i => i.Id).ToListAsync();

                foreach (var invoiceReceipt in invoiceReceipts)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoiceReceipt.CountryCode);
                    var countryName = country != null ? country.Name : "";

                    invoiceReceipt.AddressLine2 = invoiceReceipt.City == null || invoiceReceipt.City == "" ? countryName : $"{invoiceReceipt.City}, {countryName} ";
                }
                return invoiceReceipts;
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<bool> SaveProformaInvoiceReceipt(ProformaInvoiceReceiptSaveRequest proformaInvoiceReceiptSaveRequest)
        {
            try
            {
                if (proformaInvoiceReceiptSaveRequest.Id > 0)
                {
                    var lpFProformaInvoiceReceipt = await dB_LogisticsproContext.LpFProformaInvoiceReceipt.FirstOrDefaultAsync(i => i.Id == proformaInvoiceReceiptSaveRequest.Id);
                    if (lpFProformaInvoiceReceipt != null)
                    {
                        lpFProformaInvoiceReceipt.Amount = proformaInvoiceReceiptSaveRequest.Amount;
                        lpFProformaInvoiceReceipt.PaymentMethodId = proformaInvoiceReceiptSaveRequest.PaymentMethod;
                        lpFProformaInvoiceReceipt.Remark = proformaInvoiceReceiptSaveRequest.Remark;
                        lpFProformaInvoiceReceipt.PaymentDate = proformaInvoiceReceiptSaveRequest.ReceiptDate;
                        lpFProformaInvoiceReceipt.UpdatedBy = proformaInvoiceReceiptSaveRequest.UserId;
                        lpFProformaInvoiceReceipt.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpFProformaInvoiceReceipt).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                var proformaInvoiceReceiptPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "ProformaInvoiceReceiptPrefix");
                var currentProformaInvoiceReceiptNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentProformaInvoiceReceiptNumber");
                string proformaInvoiceReceiptCode = string.Empty;
                int nextNumber = 0;

                if (proformaInvoiceReceiptPrefix != null && currentProformaInvoiceReceiptNumber != null)
                {
                    nextNumber = int.Parse(currentProformaInvoiceReceiptNumber.SystemValue) + 1;
                    proformaInvoiceReceiptCode = $"{proformaInvoiceReceiptPrefix.SystemValue} {nextNumber.ToString("D5")}";

                }
                LpFProformaInvoiceReceipt proformaInvoiceReceipt = new LpFProformaInvoiceReceipt();

                proformaInvoiceReceipt.ReceiptCode = proformaInvoiceReceiptCode;
                proformaInvoiceReceipt.ProformaInvoiceId = proformaInvoiceReceiptSaveRequest.ProformaInvoiceId;
                proformaInvoiceReceipt.JobCardId = proformaInvoiceReceiptSaveRequest.JobCardId;
                proformaInvoiceReceipt.Amount = proformaInvoiceReceiptSaveRequest.Amount;
                proformaInvoiceReceipt.PaymentMethodId = proformaInvoiceReceiptSaveRequest.PaymentMethod;
                proformaInvoiceReceipt.Remark = proformaInvoiceReceiptSaveRequest.Remark;
                proformaInvoiceReceipt.PaymentDate = proformaInvoiceReceiptSaveRequest.ReceiptDate;
                proformaInvoiceReceipt.IsAllocatedToInvoice = false;
                proformaInvoiceReceipt.CreatedBy = proformaInvoiceReceiptSaveRequest.UserId;
                proformaInvoiceReceipt.CreatedDate = DateTime.UtcNow;
                proformaInvoiceReceipt.UpdatedBy = proformaInvoiceReceiptSaveRequest.UserId;
                proformaInvoiceReceipt.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpFProformaInvoiceReceipt.Add(proformaInvoiceReceipt);
                await dB_LogisticsproContext.SaveChangesAsync();

                await UpdateProformaInvoiceStatus(proformaInvoiceReceiptSaveRequest.ProformaInvoiceId.Value);

                currentProformaInvoiceReceiptNumber.SystemValue = nextNumber.ToString();
                currentProformaInvoiceReceiptNumber.UpdatedBy = proformaInvoiceReceiptSaveRequest.UserId;
                currentProformaInvoiceReceiptNumber.UpdatedDate = DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentProformaInvoiceReceiptNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }
        public async Task<bool> RemoveProformaInvoiceReceipt(int id)
        {
            var proformaInvoiceReceipt = await dB_LogisticsproContext.LpFProformaInvoiceReceipt.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpFProformaInvoiceReceipt.Remove(proformaInvoiceReceipt);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }

        public async Task<Invoice> GetInvoice(int id)
        {
            try
            {
                var countries = await GetCountries();
                var invoice = await (from i in dB_LogisticsproContext.LpFInvoice
                                         join lpc in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals lpc.Id
                                         join s in dB_LogisticsproContext.LpMInvoiceStatus on i.StatusId equals s.Id
                                         where i.Id==id
                                         select new Invoice
                                         {
                                             Id = i.Id,
                                             InvoiceCode = i.InvoiceCode,
                                             CustomerId = i.CustomerId,
                                             CustomerName = lpc.CustomerName,
                                             CustomerAddressLine1 = lpc.Address1 == null || lpc.Address1 == "" ? lpc.Address2 : $"{lpc.Address1}, {lpc.Address2} ",
                                             CustomerTrn= lpc.Trn,
                                             City = lpc.City,
                                             CountryCode = lpc.CountryCode,
                                             InvoiceDate = i.InvoiceDate,
                                             InvoiceDueDate = i.InvoiceDueDate,
                                             InvoiceAmount = i.InvoiceAmount,
                                             TransportDescription = i.TransportDescription,
                                             HotelDescription = i.HotelDescription,
                                             VisaDescription = i.VisaDescription,
                                             MiscellaneousDescription = i.MiscellaneousDescription,
                                             StatusName = s.StatusName,
                                             StatusId = s.Id
                                         }).FirstOrDefaultAsync();

                if(invoice!=null)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoice.CountryCode);
                    var countryName = country != null ? country.Name : "";

                    invoice.CustomerAddressLine2 = invoice.City == null || invoice.City == "" ? countryName : $"{invoice.City}, {countryName} ";

                    var lineItems = new LineGenerateItem();

                    var transpostBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                     join ljt in dB_LogisticsproContext.LpJTransportation on il.ContextId equals ljt.Id
                                                     join j in dB_LogisticsproContext.LpJobCard on ljt.JobCardId equals j.Id
                                                     join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                     where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Transpotation
                                                     select new JobCardTransportation
                                                     {
                                                         Id = ljt.Id,
                                                         JobCardNo = j.JobCardCode,
                                                         JobCardDescription = j.JobDescription,
                                                         JobCardId = ljt.JobCardId,
                                                         CustomerName = c.CustomerName,
                                                         BookingRef = ljt.TransportationCode,
                                                         CustomerRef = ljt.CustomerRef,
                                                         Remarks = ljt.Remarks,
                                                         DropoffLocation = ljt.DropoffLocation,
                                                         CostBaseAmount = ljt.CostBaseAmount,
                                                         CostTaxAmount = ljt.CostTaxAmount,
                                                         IsVatIncludedSell = ljt.IsVatIncludedSell,
                                                         IsVatIncludedCost = ljt.IsVatIncludedCost,
                                                         SellBaseAmount = il.SellBaseAmount,
                                                         SellTaxAmount = il.SellTaxAmount,
                                                         PickupLocation = ljt.PickupLocation,
                                                         PickupTime = ljt.PickupTime,
                                                         Adults = ljt.Adults,
                                                         Children = ljt.Children,
                                                         Infants = ljt.Infants,
                                                         VehicleType = ljt.VehicleType,
                                                         FlightNo = ljt.FlightNo,
                                                         FlightTime = ljt.FlightTime,
                                                         PaxName = ljt.PaxName,
                                                         Parking = ljt.Parking,
                                                         ParkingTaxAmount = ljt.ParkingTaxAmount,
                                                         Extras = ljt.Extras,
                                                         ExtrasTaxAmount = ljt.ExtrasTaxAmount,
                                                         Water = ljt.Water,
                                                         WaterTaxAmount = ljt.WaterTaxAmount,
                                                         ParkingSell = ljt.ParkingSell,
                                                         ParkingTaxAmountSell = ljt.ParkingTaxAmountSell,
                                                         ExtrasSell = ljt.ExtrasSell,
                                                         ExtrasTaxAmountSell = ljt.ExtrasTaxAmountSell,
                                                         WaterSell = ljt.WaterSell,
                                                         WaterTaxAmountSell = ljt.WaterTaxAmountSell
                                                     }).Distinct().OrderBy(i => i.PickupTime).ToListAsync();

                    

                    foreach (var transpostBatchItem in transpostBatchItems)
                    {
                        var receiptAllocations=await dB_LogisticsproContext.LpFReceiptAllocation.Where(i=>i.InvoiceId==invoice.Id && i.ContextId== transpostBatchItem.Id && i.ContextTypeId==(int)EnumContextType.Transpotation).ToListAsync();
                        decimal receiptBalance = 0;
                        foreach (var receiptAllocation in receiptAllocations)
                        {
                            var amount= receiptAllocation.Amount==null? 0 : receiptAllocation.Amount.Value;
                            receiptBalance = receiptBalance + amount;
                        }

                        var balanceAmount = transpostBatchItem.TotalSellPrice - receiptBalance;
                        transpostBatchItem.ReceiptBalanceAmount= balanceAmount;
                        transpostBatchItem.TotalReceiptAmount= receiptBalance;
                    }

                    lineItems.Transportations.AddRange(transpostBatchItems);

                    var hotelBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                 join ljh in dB_LogisticsproContext.LpJHotel on il.ContextId equals ljh.Id
                                                 join j in dB_LogisticsproContext.LpJobCard on ljh.JobCardId equals j.Id
                                                 join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                 where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Hotel
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
                                                     CostBaseAmount = ljh.CostBaseAmount,
                                                     CostTaxAmount = ljh.CostTaxAmount,
                                                     IsVatIncludedSell = ljh.IsVatIncludedSell,
                                                     IsVatIncludedCost = ljh.IsVatIncludedCost,
                                                     SellBaseAmount = il.SellBaseAmount,
                                                     SellTaxAmount = il.SellTaxAmount,
                                                     Remarks = ljh.Remarks,
                                                     RoomType = ljh.RoomType,
                                                     HotelConfirmation = ljh.HotelConfirmation
                                                 }).Distinct().OrderBy(i => i.CheckIn).ToListAsync();



                    foreach (var hotelBatchItem in hotelBatchItems)
                    {
                        var receiptAllocations = await dB_LogisticsproContext.LpFReceiptAllocation.Where(i => i.InvoiceId == invoice.Id && i.ContextId == hotelBatchItem.Id && i.ContextTypeId == (int)EnumContextType.Hotel).ToListAsync();
                        decimal receiptBalance = 0;
                        foreach (var receiptAllocation in receiptAllocations)
                        {
                            var amount = receiptAllocation.Amount == null ? 0 : receiptAllocation.Amount.Value;
                            receiptBalance = receiptBalance + amount;
                        }

                        var balanceAmount = hotelBatchItem.TotalSellPrice - receiptBalance;
                        hotelBatchItem.ReceiptBalanceAmount = balanceAmount;
                        hotelBatchItem.TotalReceiptAmount = receiptBalance;
                    }

                    lineItems.Hotels.AddRange(hotelBatchItems);

                    var visaBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                join ljv in dB_LogisticsproContext.LpJVisa on il.ContextId equals ljv.Id
                                                join lpvt in dB_LogisticsproContext.LpMVisaType on ljv.VisaTypeId equals lpvt.Id
                                                join j in dB_LogisticsproContext.LpJobCard on ljv.JobCardId equals j.Id
                                                join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Visa
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
                                                    CreatedDate= ljv.CreatedDate
                                                }).Distinct().OrderBy(i => i.CreatedDate).ToListAsync();


                    foreach (var visaBatchItem in visaBatchItems)
                    {
                        var receiptAllocations = await dB_LogisticsproContext.LpFReceiptAllocation.Where(i => i.InvoiceId == invoice.Id && i.ContextId == visaBatchItem.Id && i.ContextTypeId == (int)EnumContextType.Visa).ToListAsync();
                        decimal receiptBalance = 0;
                        foreach (var receiptAllocation in receiptAllocations)
                        {
                            var amount = receiptAllocation.Amount == null ? 0 : receiptAllocation.Amount.Value;
                            receiptBalance = receiptBalance + amount;
                        }

                        var balanceAmount = visaBatchItem.TotalSellPrice - receiptBalance;
                        visaBatchItem.ReceiptBalanceAmount = balanceAmount;
                        visaBatchItem.TotalReceiptAmount = receiptBalance;
                    }


                    lineItems.Visa.AddRange(visaBatchItems);

                    var miscellaneousBatchItems = await (from il in dB_LogisticsproContext.LpFInvoiceLineItem
                                                         join ljm in dB_LogisticsproContext.LpJMiscellaneous on il.ContextId equals ljm.Id
                                                         join j in dB_LogisticsproContext.LpJobCard on ljm.JobCardId equals j.Id
                                                         join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                                         where il.InvoiceId == invoice.Id && il.ContextTypeId == (int)EnumContextType.Miscellaneous
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
                                                             Remarks = ljm.Remarks
                                                         }).Distinct().OrderBy(i => i.MisDate).ToListAsync();

                    foreach (var miscellaneousBatchItem in miscellaneousBatchItems)
                    {
                        var receiptAllocations = await dB_LogisticsproContext.LpFReceiptAllocation.Where(i => i.InvoiceId == invoice.Id && i.ContextId == miscellaneousBatchItem.Id && i.ContextTypeId == (int)EnumContextType.Miscellaneous).ToListAsync();
                        decimal receiptBalance = 0;
                        foreach (var receiptAllocation in receiptAllocations)
                        {
                            var amount = receiptAllocation.Amount == null ? 0 : receiptAllocation.Amount.Value;
                            receiptBalance = receiptBalance + amount;
                        }

                        var balanceAmount = miscellaneousBatchItem.TotalSellPrice - receiptBalance;
                        miscellaneousBatchItem.ReceiptBalanceAmount = balanceAmount;
                        miscellaneousBatchItem.TotalReceiptAmount = receiptBalance;
                    }

                    lineItems.Miscellaneous.AddRange(miscellaneousBatchItems);

                    invoice.InvoiceLineItems= lineItems;
                }
                return invoice == null ? new Invoice() : invoice;
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<bool> VoidInvoiceReceipt(int id)
        {
            var lpFReceipt = await dB_LogisticsproContext.LpFReceipt.FirstOrDefaultAsync(i => i.Id == id);
            if (lpFReceipt == null)
            {
                return false;
            }
            lpFReceipt.IsVoid = true;
            dB_LogisticsproContext.Entry(lpFReceipt).State = EntityState.Modified;
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }
        public async Task<List<Receipt>> GetReceipts(int id)
        {
            try
            {
                var countries = await GetCountries();
                var invoiceReceipts = await (from ir in dB_LogisticsproContext.LpFReceipt
                                             join i in dB_LogisticsproContext.LpFInvoice on ir.InvoiceId equals i.Id
                                             join c in dB_LogisticsproContext.LpMCustomer on i.CustomerId equals c.Id
                                             join pm in dB_LogisticsproContext.LpMPaymentMethod on ir.PaymentMethodId equals pm.Id
                                             where (ir.InvoiceId == id)
                                             select new Receipt
                                             {
                                                 Id = ir.Id,
                                                 PaymentMethod = pm.PaymentMethodName,
                                                 InvoiceId = i.Id,
                                                 PaymentDate = ir.PaymentDate,
                                                 PaymentMethodId = ir.PaymentMethodId,
                                                 Remark = ir.Remark,
                                                 Amount = ir.Amount,
                                                 InvoiceCode = i.InvoiceCode,
                                                 ReceiptCode = ir.ReceiptCode,
                                                 CustomerName = c.CustomerName,
                                                 CountryCode = c.CountryCode,
                                                 City = c.City,
                                                 Status=ir.IsVoid==true?"Void":"Active",
                                                 AddressLine1 = c.Address1 == null || c.Address1 == "" ? c.Address2 : $"{c.Address1}, {c.Address2} ",
                                             }).OrderByDescending(i => i.Id).ToListAsync();

                foreach (var invoiceReceipt in invoiceReceipts)
                {
                    var country = countries.FirstOrDefault(i => i.Ccn3 == invoiceReceipt.CountryCode);
                    var countryName = country != null ? country.Name : "";

                    invoiceReceipt.AddressLine2 = invoiceReceipt.City == null || invoiceReceipt.City == "" ? countryName : $"{invoiceReceipt.City}, {countryName} ";
                }
                return invoiceReceipts;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> SaveReceipt(ReceiptSaveRequest receiptSaveRequest)
        {
            try
            {
                if (receiptSaveRequest.Id > 0)
                {
                    var lpFReceipt = await dB_LogisticsproContext.LpFReceipt.FirstOrDefaultAsync(i => i.Id == receiptSaveRequest.Id);
                    if (lpFReceipt != null)
                    {
                        lpFReceipt.Amount = receiptSaveRequest.Amount;
                        lpFReceipt.PaymentMethodId = receiptSaveRequest.PaymentMethod;
                        lpFReceipt.Remark = receiptSaveRequest.Remark;
                        lpFReceipt.PaymentDate = receiptSaveRequest.ReceiptDate;
                        lpFReceipt.UpdatedBy = receiptSaveRequest.UserId;
                        lpFReceipt.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpFReceipt).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                var paymentReceiptPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "PaymentReceiptPrefix");
                var currentPaymentReceiptNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentPaymentReceiptNumber");
                string paymentReceiptCode = string.Empty;
                int nextNumber = 0;

                if (paymentReceiptPrefix != null && currentPaymentReceiptNumber != null)
                {
                    nextNumber = int.Parse(currentPaymentReceiptNumber.SystemValue) + 1;
                    paymentReceiptCode = $"{paymentReceiptPrefix.SystemValue} {nextNumber.ToString("D5")}";

                }
                LpFReceipt receipt = new LpFReceipt();

                receipt.ReceiptCode = paymentReceiptCode;
                receipt.InvoiceId = receiptSaveRequest.InvoiceId;
                receipt.Amount = receiptSaveRequest.Amount;
                receipt.PaymentMethodId = receiptSaveRequest.PaymentMethod;
                receipt.Remark = receiptSaveRequest.Remark;
                receipt.PaymentDate = receiptSaveRequest.ReceiptDate;
                receipt.IsAllocated = false;
                receipt.IsVoid = false;
                receipt.CreatedBy = receiptSaveRequest.UserId;
                receipt.CreatedDate = DateTime.UtcNow;
                receipt.UpdatedBy = receiptSaveRequest.UserId;
                receipt.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpFReceipt.Add(receipt);
                await dB_LogisticsproContext.SaveChangesAsync();

                foreach (var records in receiptSaveRequest.UpdateRecords)
                {
                    if (records.ServiceType == "Transportation")
                    {
                        LpFReceiptAllocation receiptAllocation = new LpFReceiptAllocation();
                        receiptAllocation.Amount = records.AllocatedAmount;
                        receiptAllocation.ReceiptId = receipt.Id;
                        receiptAllocation.InvoiceId = receipt.InvoiceId;
                        receiptAllocation.ContextId = records.Id;
                        receiptAllocation.ContextTypeId = (int)EnumContextType.Transpotation;
                        receiptAllocation.CreatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.CreatedDate = DateTime.UtcNow;
                        receiptAllocation.UpdatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFReceiptAllocation.Add(receiptAllocation);
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    if (records.ServiceType == "Hotel")
                    {
                        LpFReceiptAllocation receiptAllocation = new LpFReceiptAllocation();
                        receiptAllocation.Amount = records.AllocatedAmount;
                        receiptAllocation.ReceiptId = receipt.Id;
                        receiptAllocation.InvoiceId = receipt.InvoiceId;
                        receiptAllocation.ContextId = records.Id;
                        receiptAllocation.ContextTypeId = (int)EnumContextType.Hotel;
                        receiptAllocation.CreatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.CreatedDate = DateTime.UtcNow;
                        receiptAllocation.UpdatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFReceiptAllocation.Add(receiptAllocation);
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    if (records.ServiceType == "Visa")
                    {
                        LpFReceiptAllocation receiptAllocation = new LpFReceiptAllocation();
                        receiptAllocation.Amount = records.AllocatedAmount;
                        receiptAllocation.ReceiptId = receipt.Id;
                        receiptAllocation.InvoiceId = receipt.InvoiceId;
                        receiptAllocation.ContextId = records.Id;
                        receiptAllocation.ContextTypeId = (int)EnumContextType.Visa;
                        receiptAllocation.CreatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.CreatedDate = DateTime.UtcNow;
                        receiptAllocation.UpdatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFReceiptAllocation.Add(receiptAllocation);
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                    if (records.ServiceType == "Miscellaneous")
                    {
                        LpFReceiptAllocation receiptAllocation = new LpFReceiptAllocation();
                        receiptAllocation.Amount = records.AllocatedAmount;
                        receiptAllocation.ReceiptId = receipt.Id;
                        receiptAllocation.InvoiceId = receipt.InvoiceId;
                        receiptAllocation.ContextId = records.Id;
                        receiptAllocation.ContextTypeId = (int)EnumContextType.Miscellaneous;
                        receiptAllocation.CreatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.CreatedDate = DateTime.UtcNow;
                        receiptAllocation.UpdatedBy = receiptSaveRequest.UserId;
                        receiptAllocation.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.LpFReceiptAllocation.Add(receiptAllocation);
                        await dB_LogisticsproContext.SaveChangesAsync();
                    }

                }

                await UpdateInvoiceStatus(receiptSaveRequest.InvoiceId.Value);

                currentPaymentReceiptNumber.SystemValue = nextNumber.ToString();
                currentPaymentReceiptNumber.UpdatedBy = receiptSaveRequest.UserId;
                currentPaymentReceiptNumber.UpdatedDate = DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentPaymentReceiptNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }

        private async Task UpdateInvoiceStatus(int invoiceId)
        {
            var invoice=await dB_LogisticsproContext.LpFInvoice.FirstOrDefaultAsync(i=>i.Id==invoiceId);
            if (invoice!=null)
            {
                var allocatedReceipts=await dB_LogisticsproContext.LpFReceiptAllocation.Where(i=>i.InvoiceId==invoiceId).ToListAsync();
                
                decimal receiptBalance = 0;
                foreach (var allocatedReceipt in allocatedReceipts)
                {
                    receiptBalance = receiptBalance + allocatedReceipt.Amount.Value;
                }

                if(invoice.InvoiceAmount> receiptBalance)
                {
                    invoice.StatusId = (int)EnumInvoiceStatus.PartiallyPaid;
                    dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                }

                if (invoice.InvoiceAmount <= receiptBalance)
                {
                    invoice.StatusId = (int)EnumInvoiceStatus.Paid;
                    dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }
        }

        private async Task UpdateProformaInvoiceStatus(int invoiceId)
        {
            var invoice = await dB_LogisticsproContext.LpFProformaInvoice.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice != null)
            {
                var allocatedReceipts = await dB_LogisticsproContext.LpFProformaInvoiceReceipt.Where(i => i.ProformaInvoiceId == invoiceId).ToListAsync();

                decimal receiptBalance = 0;
                foreach (var allocatedReceipt in allocatedReceipts)
                {
                    receiptBalance = receiptBalance + allocatedReceipt.Amount.Value;
                }

                if (invoice.InvoiceAmount > receiptBalance)
                {
                    invoice.StatusId = (int)EnumInvoiceStatus.PartiallyPaid;
                    dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                }

                if (invoice.InvoiceAmount <= receiptBalance)
                {
                    invoice.StatusId = (int)EnumInvoiceStatus.Paid;
                    dB_LogisticsproContext.Entry(invoice).State = EntityState.Modified;
                    await dB_LogisticsproContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<PnL>> GetPnL(List<int>? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber)
        {
            try
            {
                var jobCards = (from j in dB_LogisticsproContext.LpJobCard
                                join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
                                where (customerId == null || customerId.Count == 0 || customerId.Any(x => j.CustomerId.Value == x)) &&
                                (dateFrom == null || j.EffectiveDate >= dateFrom) &&
                                   (dateTo == null || j.EffectiveDate <= dateTo) &&
                                   (jobCardNumber == null || j.JobCardCode.Contains(jobCardNumber))
                                select new PnL
                                {
                                    JobCardId=j.Id,
                                    JobCardNo=j.JobCardCode,
                                    JobCardDescription=j.JobDescription,
                                    CustomerName=c.CustomerName
                                }).ToList();

                var jobCardLineItems = new List<JobCardLineItem>();
                var lpJobFinanceReceiptList = new List<JobCardFinanceRreceipt>();
                var lpJobFinanceVoucherList = new List<JobCardFinancePaymentVoucher>();
                var lpJobFinanceInvoiceList = new List<JobCardFinanceInvoice>();

                foreach (var jobCard in jobCards)
                {
                    jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJTransportation.Where(i => i.JobCardId == jobCard.JobCardId).Select(x => new JobCardLineItem
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

                    jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJHotel.Where(i => i.JobCardId == jobCard.JobCardId).Select(x => new JobCardLineItem
                    {
                        Id = x.Id,
                        JobCardId = x.JobCardId,
                        BookingRef = x.HotelCode,
                        CostBaseAmount = x.CostBaseAmount,
                        CostTaxAmount = x.CostTaxAmount,
                        ContextIdType = (int)EnumContextType.Hotel
                    }).ToListAsync());

                    jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJVisa.Where(i => i.JobCardId == jobCard.JobCardId).Select(x => new JobCardLineItem
                    {
                        Id = x.Id,
                        JobCardId = x.JobCardId,
                        BookingRef = x.VisaCode,
                        CostBaseAmount = x.CostBaseAmount,
                        CostTaxAmount = x.CostTaxAmount,
                        ContextIdType = (int)EnumContextType.Visa
                    }).ToListAsync());

                    jobCardLineItems.AddRange(await dB_LogisticsproContext.LpJMiscellaneous.Where(i => i.JobCardId == jobCard.JobCardId).Select(x => new JobCardLineItem
                    {
                        Id = x.Id,
                        JobCardId = x.JobCardId,
                        BookingRef = x.MiscellaneousCode,
                        CostBaseAmount = x.CostBaseAmount,
                        CostTaxAmount = x.CostTaxAmount,
                        ContextIdType = (int)EnumContextType.Miscellaneous
                    }).ToListAsync());
                }

                foreach (var jobCardLineItem in jobCardLineItems)
                {
                    var lpJobFinanceReceipts = await (from r in dB_LogisticsproContext.LpFReceipt
                                                      join i in dB_LogisticsproContext.LpFInvoice on r.InvoiceId equals i.Id
                                                      join ra in dB_LogisticsproContext.LpFReceiptAllocation on r.Id equals ra.ReceiptId
                                                      where ra.ContextId == jobCardLineItem.Id && ra.ContextTypeId == jobCardLineItem.ContextIdType && i.StatusId!=(int)EnumInvoiceStatus.Void
                                                      && r.IsVoid==false
                                                      select new JobCardFinanceRreceipt
                                                      {
                                                          Id = r.Id,
                                                          JobCardId= jobCardLineItem.JobCardId,
                                                          Amount = r.Amount,
                                                          Remarks = r.Remark,
                                                          ReceiptDate = r.PaymentDate,
                                                          ReceiptCode = r.ReceiptCode,
                                                          InvoiceNo = i.InvoiceCode
                                                      }).Distinct().ToListAsync();

                    lpJobFinanceReceiptList.AddRange(lpJobFinanceReceipts);


                    var lpJobFinanceVouchers = await (from pv in dB_LogisticsproContext.LpFPaymentVoucher
                                                      join v in dB_LogisticsproContext.LpMVender on pv.VendorId equals v.Id
                                                      join pvi in dB_LogisticsproContext.LpFPaymentVoucherLineItem on pv.Id equals pvi.PaymentVoucherId
                                                      where pvi.ContextId == jobCardLineItem.Id && pvi.ContextTypeId == jobCardLineItem.ContextIdType
                                                      select new JobCardFinancePaymentVoucher
                                                      {
                                                          Id = pv.Id,
                                                          JobCardId = jobCardLineItem.JobCardId,
                                                          Amount = jobCardLineItem.TotalCostPrice,
                                                          Remarks = pv.Remark,
                                                          VendorName = v.VenderName,
                                                          VoucherCode = pv.PaymentVoucherCode,
                                                          VoucherDate = pv.VoucherDate
                                                      }).ToListAsync();

                    lpJobFinanceVoucherList.AddRange(lpJobFinanceVouchers);

                    var lpJobFinanceInvoice = await (from i in dB_LogisticsproContext.LpFInvoice
                                                     join il in dB_LogisticsproContext.LpFInvoiceLineItem on i.Id equals il.InvoiceId
                                                     where il.ContextId == jobCardLineItem.Id && il.ContextTypeId == jobCardLineItem.ContextIdType && i.StatusId != (int)EnumInvoiceStatus.Void
                                                     select new JobCardFinanceInvoice
                                                     {
                                                         Id = i.Id,
                                                         JobCardId = jobCardLineItem.JobCardId,
                                                         Amount = i.InvoiceAmount,
                                                         Remarks = i.Remark,
                                                         InvoiceCode = i.InvoiceCode,
                                                         InvoiceDate = i.InvoiceDate,
                                                         InvoiceDueDate = i.InvoiceDueDate
                                                     }).ToListAsync();

                    lpJobFinanceInvoiceList.AddRange(lpJobFinanceInvoice);
                }

                var jobFinanceReceipts = (from p in lpJobFinanceReceiptList
                                          group p by new { p.Id,p.JobCardId, p.ReceiptCode, p.Amount, p.Remarks, p.ReceiptDate, p.InvoiceNo } into g
                                          select new JobCardFinanceRreceipt
                                          {
                                              Id = g.Key.Id,
                                              JobCardId=g.Key.JobCardId,
                                              Amount = g.Key.Amount,
                                              Remarks = g.Key.Remarks,
                                              ReceiptDate = g.Key.ReceiptDate,
                                              ReceiptCode = g.Key.ReceiptCode,
                                              InvoiceNo = g.Key.InvoiceNo
                                          }).ToList();

                var jobFinanceInvoices = (from p in lpJobFinanceInvoiceList
                                          group p by new { p.Id, p.JobCardId, p.InvoiceCode, p.Amount, p.Remarks, p.InvoiceDate, p.InvoiceDueDate } into g
                                          select new JobCardFinanceInvoice
                                          {
                                              Id = g.Key.Id,
                                              JobCardId = g.Key.JobCardId,
                                              Amount = g.Key.Amount,
                                              Remarks = g.Key.Remarks,
                                              InvoiceDate = g.Key.InvoiceDate,
                                              InvoiceDueDate = g.Key.InvoiceDueDate,
                                              InvoiceCode = g.Key.InvoiceCode
                                          }).ToList();

                var paymentVouchers = (from p in lpJobFinanceVoucherList
                                       group p by new { p.Id, p.JobCardId, p.Remarks, p.VendorName, p.VoucherDate, p.VoucherCode } into g
                                       select new JobCardFinancePaymentVoucher
                                       {
                                           Id = g.Key.Id,
                                           JobCardId = g.Key.JobCardId,
                                           Amount = g.Sum(i => i.Amount),
                                           Remarks = g.Key.Remarks,
                                           VendorName = g.Key.VendorName,
                                           VoucherCode = g.Key.VoucherCode,
                                           VoucherDate = g.Key.VoucherDate
                                       }).ToList();

                foreach (var jobCard in jobCards)
                {
                    var jobCardReceiptAmount = jobFinanceReceipts.Where(i => i.JobCardId == jobCard.JobCardId).Sum(i => i.Amount);
                    var invoiceAmount = jobFinanceInvoices.Where(i => i.JobCardId == jobCard.JobCardId).Sum(i => i.Amount);
                    var paymentVoucherAmount = paymentVouchers.Where(i => i.JobCardId == jobCard.JobCardId).Sum(i => i.Amount);

                    jobCard.ReceiptAmount = jobCardReceiptAmount;
                    jobCard.InvoiceAmount = invoiceAmount;
                    jobCard.PaymentVoucherAmount = paymentVoucherAmount;
                }


                return jobCards;
            }
            catch (Exception e)
            {

                throw;
            }
        }

    }
}
