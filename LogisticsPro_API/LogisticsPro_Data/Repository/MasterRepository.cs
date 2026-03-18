using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LogisticsPro_Data.Repository
{
    public class MasterRepository:IMasterRepository
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public MasterRepository(DB_LogisticsproContext dB_LogisticsproContext) {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> customers= new List<Customer>();
            var customerList= await dB_LogisticsproContext.LpMCustomer.OrderByDescending(i=>i.Id).ToListAsync();

            customerList.ForEach(customer =>
            {
                customers.Add(new Customer
                {
                    Id= customer.Id,
                    CustomerCode= customer.CustomerCode,
                    ContactNumber= customer.ContactNumber,
                    Trn= customer.Trn,
                    Address1 = customer.Address1,
                    Address2 = customer.Address2,
                    City= customer.City,
                    CountryCode= customer.CountryCode,
                    ContactPersonName= customer.ContactPersonName,
                    CustomerName= customer.CustomerName,
                    Email = customer.Email
                });
            });

            return customers;
        }

        public async Task<bool> SaveCustomer(CustomerSaveRequest customerSaveRequest)
        {
            try
            {
                if (customerSaveRequest.Id > 0)
                {
                    var lpCustomer = await dB_LogisticsproContext.LpMCustomer.FirstOrDefaultAsync(i => i.Id == customerSaveRequest.Id);
                    if (lpCustomer != null)
                    {
                        lpCustomer.CustomerName = customerSaveRequest.CustomerName;
                        lpCustomer.ContactPersonName = customerSaveRequest.ContactPersonName;
                        lpCustomer.Email = customerSaveRequest.Email;
                        lpCustomer.ContactNumber = customerSaveRequest.ContactNumber;
                        lpCustomer.Trn = customerSaveRequest.Trn;
                        lpCustomer.Address1 = customerSaveRequest.Address1;
                        lpCustomer.Address2 = customerSaveRequest.Address2;
                        lpCustomer.City= customerSaveRequest.City;
                        lpCustomer.CountryCode= customerSaveRequest.CountryCode;
                        lpCustomer.UpdatedBy = customerSaveRequest.UserId;
                        lpCustomer.UpdatedDate = DateTime.UtcNow;

                        dB_LogisticsproContext.Entry(lpCustomer).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                var customerPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CustomerPrefix");
                var currentCustomerNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentCustomerNumber");
                string customerPrefixCode = string.Empty;
                int nextNumber = 0;

                if (customerPrefix != null && currentCustomerNumber != null)
                {
                    nextNumber = int.Parse(currentCustomerNumber.SystemValue) + 1;
                    customerPrefixCode = $"{customerPrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpMCustomer customer = new LpMCustomer();
                customer.CustomerCode = customerPrefixCode;
                customer.CustomerName = customerSaveRequest.CustomerName;
                customer.ContactPersonName = customerSaveRequest.ContactPersonName;
                customer.Email = customerSaveRequest.Email;
                customer.ContactNumber = customerSaveRequest.ContactNumber;
                customer.Trn = customerSaveRequest.Trn;
                customer.Address1 = customerSaveRequest.Address1;
                customer.Address2 = customerSaveRequest.Address2;
                customer.City = customerSaveRequest.City;
                customer.CountryCode = customerSaveRequest.CountryCode;
                customer.CreatedBy = customerSaveRequest.UserId;
                customer.CreatedDate = DateTime.UtcNow;
                customer.UpdatedBy = customerSaveRequest.UserId;
                customer.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpMCustomer.Add(customer);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentCustomerNumber.SystemValue = nextNumber.ToString();
                currentCustomerNumber.UpdatedBy = customerSaveRequest.UserId;
                currentCustomerNumber.UpdatedDate = DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentCustomerNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }

        public async Task<bool> RemoveCustomer(long id)
        {
            var lpCustomer = await dB_LogisticsproContext.LpMCustomer.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpMCustomer.Remove(lpCustomer);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }
        public async Task<List<Vendor>> GetVendors()
        {
            List<Vendor> vendors = new List<Vendor>();
            var vendorList = await dB_LogisticsproContext.LpMVender.OrderByDescending(i=>i.Id).ToListAsync();

            vendorList.ForEach(customer =>
            {
                vendors.Add(new Vendor
                {
                    Id = customer.Id,
                    VendorCode = customer.VenderCode,
                    ContactNumber = customer.ContactNumber,
                    Trn = customer.Trn,
                    ContactPersonName = customer.ContactPersonName,
                    VendorName = customer.VenderName,
                    Email = customer.Email,
                    Address1 = customer.Address1,
                    Address2 = customer.Address2,
                    Iban = customer.Iban,
                    SwiftCode=customer.SwiftCode,
                    BankBranch = customer.BankBranch,
                    BankCode = customer.BankCode,
                    BankName = customer.BankName,
                    CountryCode = customer.CountryCode,
                    City = customer.City,
                    VendorTypeIds=customer.VendorTypeIds
                });
            });

            
            return vendors;
        }

        public async Task<bool> SaveVendor(VendorSaveRequest vendorSaveRequest)
        {
            try
            {
                if (vendorSaveRequest.Id > 0)
                {
                    var lpMVender = await dB_LogisticsproContext.LpMVender.FirstOrDefaultAsync(i => i.Id == vendorSaveRequest.Id);
                    if (lpMVender != null)
                    {
                        lpMVender.VenderName = vendorSaveRequest.VendorName;
                        lpMVender.ContactPersonName = vendorSaveRequest.ContactPersonName;
                        lpMVender.Email = vendorSaveRequest.Email;
                        lpMVender.ContactNumber = vendorSaveRequest.ContactNumber;
                        lpMVender.Trn = vendorSaveRequest.Trn;
                        lpMVender.Address1 = vendorSaveRequest.Address1;
                        lpMVender.Address2 = vendorSaveRequest.Address2;
                        lpMVender.City = vendorSaveRequest.City;
                        lpMVender.CountryCode = vendorSaveRequest.CountryCode;
                        lpMVender.BankName = vendorSaveRequest.BankName;
                        lpMVender.BankCode = vendorSaveRequest.BankCode;
                        lpMVender.BankBranch = vendorSaveRequest.BankBranch;
                        lpMVender.Iban = vendorSaveRequest.Iban;
                        lpMVender.SwiftCode = vendorSaveRequest.SwiftCode;
                        lpMVender.VendorTypeIds = vendorSaveRequest.SelectedVendorTypes;
                        lpMVender.UpdatedBy = vendorSaveRequest.UserId;
                        lpMVender.UpdatedDate = DateTime.UtcNow;
                        dB_LogisticsproContext.Entry(lpMVender).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                var vendorPrefix = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "VenderPrefix");
                var currentVendorNumber = await dB_LogisticsproContext.LpSystemConfig.FirstOrDefaultAsync(o => o.SystemCode == "CurrentVendorNumber");
                string vendorPrefixCode = string.Empty;
                int nextNumber = 0;

                if (vendorPrefix != null && currentVendorNumber != null)
                {
                    nextNumber = int.Parse(currentVendorNumber.SystemValue) + 1;
                    vendorPrefixCode = $"{vendorPrefix.SystemValue} {nextNumber.ToString("D4")}";
                }

                LpMVender vender = new LpMVender();
                vender.VenderCode = vendorPrefixCode;
                vender.VenderName = vendorSaveRequest.VendorName;
                vender.ContactPersonName = vendorSaveRequest.ContactPersonName;
                vender.Email = vendorSaveRequest.Email;
                vender.ContactNumber = vendorSaveRequest.ContactNumber;
                vender.Trn = vendorSaveRequest.Trn;
                vender.Address1 = vendorSaveRequest.Address1;
                vender.Address2 = vendorSaveRequest.Address2;
                vender.City = vendorSaveRequest.City;
                vender.CountryCode = vendorSaveRequest.CountryCode;
                vender.BankName = vendorSaveRequest.BankName;
                vender.BankCode = vendorSaveRequest.BankCode;
                vender.BankBranch = vendorSaveRequest.BankBranch;
                vender.Iban = vendorSaveRequest.Iban;
                vender.SwiftCode = vendorSaveRequest.SwiftCode;
                vender.VendorTypeIds = vendorSaveRequest.SelectedVendorTypes;
                vender.CreatedBy = vendorSaveRequest.UserId;
                vender.CreatedDate = DateTime.UtcNow;
                vender.UpdatedBy = vendorSaveRequest.UserId;
                vender.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpMVender.Add(vender);
                await dB_LogisticsproContext.SaveChangesAsync();

                currentVendorNumber.SystemValue = nextNumber.ToString();
                currentVendorNumber.UpdatedBy = vendorSaveRequest.UserId;
                currentVendorNumber.UpdatedDate = DateTime.UtcNow;
                dB_LogisticsproContext.Entry(currentVendorNumber).State = EntityState.Modified;
                await dB_LogisticsproContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }

        public async Task<bool> RemoveVendor(long id)
        {
            var lpMVender = await dB_LogisticsproContext.LpMVender.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpMVender.Remove(lpMVender);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
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

        public async Task<List<JobCard>> GetJobCards(int customerId)
        {
            List<JobCard> jobCards = new List<JobCard>();
            var lpJobCardList = await dB_LogisticsproContext.LpJobCard.Where(i => i.StatusId==1 && i.CustomerId==customerId).ToListAsync();

            lpJobCardList.ForEach(jobCard =>
            {
                jobCards.Add(new JobCard
                {
                    Id = jobCard.Id,
                    JobCardCode = jobCard.JobCardCode,
                    JobCardDescription = jobCard.JobDescription
                });
            });

            return jobCards;
        }

        public async Task<List<SystemRole>> GetSystemRoles()
        {
            List<SystemRole> systemRoles = new List<SystemRole>();
            var lpMSystemRoleList = await dB_LogisticsproContext.LpMSystemRole.OrderByDescending(i => i.Id).ToListAsync();

            lpMSystemRoleList.ForEach(r =>
            {
                systemRoles.Add(new SystemRole
                {
                    Id = r.Id,
                    RoleName=r.RoleName
                });
            });

            return systemRoles;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethods()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
            var lpMPaymentMethods = await dB_LogisticsproContext.LpMPaymentMethod.OrderByDescending(i => i.Id).ToListAsync();

            lpMPaymentMethods.ForEach(r =>
            {
                paymentMethods.Add(new PaymentMethod
                {
                    Id = r.Id,
                    PaymentMethodName = r.PaymentMethodName
                });
            });

            return paymentMethods;
        }

        public async Task<List<InvoiceStatus>> GetInvoiceStatus()
        {
            List<InvoiceStatus> invoiceStatuses = new List<InvoiceStatus>();
            invoiceStatuses.Add(new InvoiceStatus { Id = 0, StatusName = "All" });
            var lpMInvoiceStatuses = await dB_LogisticsproContext.LpMInvoiceStatus.OrderByDescending(i => i.Id).ToListAsync();

            lpMInvoiceStatuses.ForEach(r =>
            {
                invoiceStatuses.Add(new InvoiceStatus
                {
                    Id = r.Id,
                    StatusName = r.StatusName
                });
            });

            return invoiceStatuses;
        }

        public async Task<List<User>> GetUsers()
        {
            return await (from u in dB_LogisticsproContext.LpMUser
                          join r in dB_LogisticsproContext.LpMSystemRole on u.RoleId equals r.Id
                          select new User
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                              Password=u.Password,
                              RoleId= u.RoleId,
                              StatusId= u.StatusId,
                              RoleName = r.RoleName,
                              StatusName = u.StatusId == 1 ? "Active" : "Deactivated"
                          }).ToListAsync();
        }

        public async Task<List<JobCardTransportation>> GetTransports()
        {
            var lpJobTransportation = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                             select new JobCardTransportation
                                             {
                                                 Id = lpt.Id,
                                                 TransportationCode = lpt.TransportationCode,
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
                                                 Remarks = lpt.Remarks,
                                                 CostTaxAmount = lpt.CostTaxAmount,
                                                 SellTaxAmount = lpt.SellTaxAmount,
                                                 CostBaseAmount = lpt.CostBaseAmount,
                                                 SellBaseAmount = lpt.SellBaseAmount,
                                                 Parking = lpt.Parking,
                                                 ParkingTaxAmount= lpt.ParkingTaxAmount,
                                                 Water = lpt.Water,
                                                 WaterTaxAmount= lpt.WaterTaxAmount,
                                                 Extras = lpt.Extras,
                                                 ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                                 ParkingSell = lpt.ParkingSell,
                                                 ParkingTaxAmountSell= lpt.ParkingTaxAmountSell,
                                                 WaterSell = lpt.WaterSell,
                                                 WaterTaxAmountSell= lpt.WaterTaxAmountSell,
                                                 ExtrasSell = lpt.ExtrasSell,
                                                 ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                                 IsInvoiced = lpt.IsInvoiced,
                                                 IsBatched = lpt.IsBatched,
                                             }).OrderBy(i => i.PickupTime).ToListAsync();


            return lpJobTransportation;
        }

        public async Task<List<JobCardHotel>> GetHotels()
        {
            var lpJobHotels = await (from lph in dB_LogisticsproContext.LpJHotel
                                     select new JobCardHotel
                                     {
                                         Id = lph.Id,
                                         HotelCode = lph.HotelCode,
                                         PaxName = lph.PaxName,
                                         CheckIn = lph.CheckIn,
                                         CheckOut = lph.CheckOut,
                                         Adults = lph.Adults,
                                         Children = lph.Children,
                                         Infants = lph.Infants,
                                         HotelName = lph.HotelName,
                                         CostBaseAmount = lph.CostBaseAmount,
                                         CostTaxAmount = lph.CostTaxAmount,
                                         SellBaseAmount = lph.SellBaseAmount,
                                         SellTaxAmount = lph.SellTaxAmount,
                                         Remarks = lph.Remarks,
                                         RoomType = lph.RoomType,
                                         HotelConfirmation = lph.HotelConfirmation,
                                         IsInvoiced = lph.IsInvoiced,
                                     }).OrderBy(i => i.CheckIn).ToListAsync();
            return lpJobHotels;
        }

        public async Task<bool> SaveUser(UserSaveRequest userSaveRequest)
        {
            try
            {
                if (userSaveRequest.Id > 0)
                {
                    var lpMUser = await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(i => i.Id == userSaveRequest.Id);
                    if (lpMUser != null)
                    {
                        lpMUser.UserName = userSaveRequest.UserName;
                        lpMUser.Password = userSaveRequest.Password;
                        lpMUser.RoleId = userSaveRequest.RoleId;
                        lpMUser.StatusId = userSaveRequest.StatusId;

                        lpMUser.UpdatedBy = userSaveRequest.UserId;
                        lpMUser.UpdatedDate = DateTime.UtcNow;
                        dB_LogisticsproContext.Entry(lpMUser).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

                LpMUser user = new LpMUser();
                user.UserName= userSaveRequest.UserName;
                user.Password= userSaveRequest.Password;
                user.StatusId = userSaveRequest.StatusId;
                user.RoleId = userSaveRequest.RoleId;
                user.CreatedBy = userSaveRequest.UserId;
                user.CreatedDate = DateTime.UtcNow;
                user.UpdatedBy = userSaveRequest.UserId;
                user.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpMUser.Add(user);
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }

        public async Task<bool> IsUserAlreadyExsitis(string userName)
        {
            var user= await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(x => x.UserName == userName);
            return user!=null?true:false;
        }

        public async Task<User?> GetUserByLoginDetails(string userName, string password)
        {
            var user = await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(x => x.UserName.ToLower().Trim() == userName.ToLower().Trim() && x.Password == password);
            return user != null ? new User { Id = user.Id, RoleId = user.RoleId.Value, StatusId = user.StatusId.Value, UserName = user.UserName } : null;
        }

        public async Task<bool> UpdateLastLogin(int userId)
        {
            var user = await dB_LogisticsproContext.LpMUser.FirstOrDefaultAsync(x => x.Id == userId);
            user.LastLoginDate = DateTime.UtcNow;
            user.UpdatedDate= DateTime.UtcNow;
            user.UpdatedBy = userId;
            dB_LogisticsproContext.Entry(user).State = EntityState.Modified;
            await dB_LogisticsproContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Event>> GetEvents()
        {
            
            try
            {
                var eventList = await (from e in dB_LogisticsproContext.LpMEvent
                                       join c in dB_LogisticsproContext.LpMCustomer on e.CustomerId equals c.Id
                                       select new Event
                                       {
                                           Id = e.Id,
                                           EventName = e.EventName,
                                           EventFromDate = e.EventFromDate,
                                           EventToDate = e.EventToDate,
                                           EventTypeId = e.EventType,
                                           Remark = e.Remark,
                                           Customerid = e.CustomerId,
                                           CustomerName = c.CustomerName
                                       }).ToListAsync();

                eventList.ForEach(i => i.EventType = GetEventName(i.EventTypeId));

                return eventList;
            }
            catch (Exception e)
            {

                throw;
            }
           

        }

        private string GetEventName(int? eventId)
        {
            if (eventId == 1)
            {
                return "Wedding";
            }

            if (eventId == 2)
            {
                return "Events";
            }

            return string.Empty;
        }

        public async Task<bool> SaveEvent(EventSaveRequest eventSaveRequest)
        {
            try
            {
                if (eventSaveRequest.Id > 0)
                {
                    var lpMEvent = await dB_LogisticsproContext.LpMEvent.FirstOrDefaultAsync(i => i.Id == eventSaveRequest.Id);
                    if (lpMEvent != null)
                    {
                        lpMEvent.EventName= eventSaveRequest.EventName;
                        lpMEvent.EventFromDate= eventSaveRequest.EventFromDate;
                        lpMEvent.EventToDate= eventSaveRequest.EventToDate;
                        lpMEvent.EventType = eventSaveRequest.EventTypeId;
                        lpMEvent.CustomerId= eventSaveRequest.CustomerId;
                        lpMEvent.Remark=eventSaveRequest.Remark;
                        lpMEvent.UpdatedBy = eventSaveRequest.UserId;
                        lpMEvent.UpdatedDate = DateTime.UtcNow;
                        dB_LogisticsproContext.Entry(lpMEvent).State = EntityState.Modified;
                        await dB_LogisticsproContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }


                LpMEvent mEvent = new LpMEvent();
                mEvent.EventName= eventSaveRequest.EventName;
                mEvent.EventFromDate = eventSaveRequest.EventFromDate;
                mEvent.EventToDate = eventSaveRequest.EventToDate;
                mEvent.EventType = eventSaveRequest.EventTypeId;
                mEvent.CustomerId = eventSaveRequest.CustomerId;
                mEvent.Remark = eventSaveRequest.Remark;
                mEvent.CreatedBy = eventSaveRequest.UserId;
                mEvent.CreatedDate = DateTime.UtcNow;
                mEvent.UpdatedBy = eventSaveRequest.UserId;
                mEvent.UpdatedDate = DateTime.UtcNow;

                dB_LogisticsproContext.LpMEvent.Add(mEvent);
                await dB_LogisticsproContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {

                throw;
            }
            return false;

        }

        public async Task<bool> RemoveEvent(long id)
        {
            var lpMEvent = await dB_LogisticsproContext.LpMEvent.FirstOrDefaultAsync(i => i.Id == id);
            dB_LogisticsproContext.LpMEvent.Remove(lpMEvent);
            await dB_LogisticsproContext.SaveChangesAsync();

            return true;
        }

        public async Task<Dashboard> GetDashboard(int userId)
        {
            Dashboard dashboard = new Dashboard();
            var currentDate = DateTime.UtcNow;


            var eventList = await (from e in dB_LogisticsproContext.LpMEvent
                                   join c in dB_LogisticsproContext.LpMCustomer on e.CustomerId equals c.Id
                                   select new Event
                                   {
                                       Id = e.Id,
                                       EventName = e.EventName,
                                       EventFromDate = e.EventFromDate,
                                       EventToDate = e.EventToDate,
                                       EventTypeId = e.EventType,
                                       Remark = e.Remark,
                                       Customerid = e.CustomerId,
                                       CustomerName = c.CustomerName
                                   }).ToListAsync();

            eventList.ForEach(i => i.EventType = GetEventName(i.EventTypeId));

            dashboard.Events = eventList;

            List<JobCardTransportation> unBatchTransfers = new List<JobCardTransportation>();

            var unbatchTransfersList = await dB_LogisticsproContext.LpJTransportation.Where(i => i.IsBatched == false && i.PickupTime != null).OrderByDescending(i => i.PickupTime).ToListAsync();

            unbatchTransfersList.ForEach(i =>
            {
                unBatchTransfers.Add(new JobCardTransportation { Id = i.Id, TransportationCode = i.TransportationCode, PickupTime = i.PickupTime });
            });

            dashboard.UnBatchTransfers = unBatchTransfers.Take(10).ToList();


            List<JobCardTransportation> upcomingTranspotation = new List<JobCardTransportation>();

            var upcomingTranspotationList = await dB_LogisticsproContext.LpJTransportation.Where(i => i.PickupTime != null).OrderByDescending(i => i.PickupTime).ToListAsync();

            upcomingTranspotationList.ForEach(i =>
            {
                upcomingTranspotation.Add(new JobCardTransportation { Id = i.JobCardId.Value, TransportationCode = i.TransportationCode, PickupTime = i.PickupTime });
            });

            dashboard.UpcomingTransfers = upcomingTranspotation.Take(10).ToList();

            List<JobCardHotel> upcomingHotel = new List<JobCardHotel>();

            var upcomingHotelList = await dB_LogisticsproContext.LpJHotel.Where(i => i.CheckIn != null).OrderByDescending(i => i.CheckIn).ToListAsync();

            upcomingHotelList.ForEach(i =>
            {
                upcomingHotel.Add(new JobCardHotel { Id = i.JobCardId.Value, HotelCode = i.HotelCode, CheckIn = i.CheckIn });
            });

            dashboard.UpcomingHotels = upcomingHotel.Take(10).ToList();


            List<JobCardVisa> upcomingVisas = new List<JobCardVisa>();

            var upcomingVisaList = await dB_LogisticsproContext.LpJVisa.Where(i => i.CreatedDate != null).OrderByDescending(i => i.CreatedDate).ToListAsync();

            upcomingVisaList.ForEach(i =>
            {
                upcomingVisas.Add(new JobCardVisa { Id = i.JobCardId.Value, VisaCode = i.VisaCode, CreatedDate = i.CreatedDate });
            });

            dashboard.UpcomingVisas = upcomingVisas.Take(10).ToList();



            List<JobCardMiscellaneous> upcomingMis = new List<JobCardMiscellaneous>();

            var upcomingMisList = await dB_LogisticsproContext.LpJMiscellaneous.Where(i => i.MisDate != null).OrderByDescending(i => i.MisDate).ToListAsync();

            upcomingMisList.ForEach(i =>
            {
                upcomingMis.Add(new JobCardMiscellaneous { Id = i.JobCardId.Value, MiscellaneousCode = i.MiscellaneousCode, MisDate = i.MisDate });
            });

            dashboard.UpcomingMiscellaneours = upcomingMis.Take(10).ToList();

            //------------------
            var monthStartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

            var jobCardsMonthly = await dB_LogisticsproContext.LpJobCard.Where(i => i.EffectiveDate.Value.Month== currentDate.Month).ToListAsync();

            //(from j in dB_LogisticsproContext.LpJobCard
            // join c in dB_LogisticsproContext.LpMCustomer on j.CustomerId equals c.Id
            // where j.EffectiveDate.Value.Month == currentDate.Month
            // select new { j, c }).ToList();// 
            dashboard.TotalJobCardMonthly = jobCardsMonthly.Count;

            var totaleTransfersMonthly = 0;
            var totaleHotelMonthly = 0;
            var totaleVisaMonthly = 0;
            var totaleMiscMonthly = 0;
            decimal totalJobCardValueMonthly = 0;

            foreach (var jobCard in jobCardsMonthly)
            {
                var trasnfers = await GetJobTransportations(jobCard.Id);
                totaleTransfersMonthly = totaleTransfersMonthly + (trasnfers == null ? 0 : trasnfers.Count);

                foreach (var trasnfer in trasnfers)
                {
                    totalJobCardValueMonthly = totalJobCardValueMonthly + trasnfer.TotalSellPrice;
                }

                var hotels = await GetJobHotels(jobCard.Id);
                totaleHotelMonthly = totaleHotelMonthly + (hotels == null ? 0 : hotels.Count);

                foreach (var hotel in hotels)
                {
                    totalJobCardValueMonthly = totalJobCardValueMonthly + hotel.TotalSellPrice;
                }

                var visas = await GetJobVisas(jobCard.Id);
                totaleVisaMonthly = totaleVisaMonthly + (visas == null ? 0 : visas.Count);

                foreach (var visa in visas)
                {
                    totalJobCardValueMonthly = totalJobCardValueMonthly + visa.TotalSellPrice;
                }

                var mises = await GetJobMiscellanea(jobCard.Id);
                totaleMiscMonthly = totaleMiscMonthly + (mises == null ? 0 : mises.Count);

                foreach (var mise in mises)
                {
                    totalJobCardValueMonthly = totalJobCardValueMonthly + mise.TotalSellPrice;
                }
            }
            dashboard.Chart_2.Monthly = new List<int?> { totaleTransfersMonthly, totaleHotelMonthly, totaleVisaMonthly, totaleMiscMonthly };
            dashboard.TotalJobCardValueMonthly = totalJobCardValueMonthly;


            var customerListMonthly = jobCardsMonthly.Where(i => i.CustomerId.HasValue).GroupBy(i => i.CustomerId).Select(i => new Customer { Id = i.Key.Value, TotalBookingCount = i.Count() }).ToList();
            List<Customer> customerMonthly = new List<Customer>();
            foreach (var customer in customerListMonthly)
            {
                var cus = await dB_LogisticsproContext.LpMCustomer.FirstOrDefaultAsync(i => i.Id == customer.Id);
                if (cus == null)
                {
                    continue;
                }

                customerMonthly.Add(new Customer
                {
                    Id= customer.Id,
                    CustomerName=cus.CustomerName,
                    TotalBookingCount=customer.TotalBookingCount
                });
            }

            dashboard.CustomersMonthly = customerMonthly.OrderByDescending(i => i.TotalBookingCount).Take(10).ToList();

            var yearStartDate = new DateTime(currentDate.Year, 1, 1);
            var yearEndDate = new DateTime(currentDate.Year, 12, 31);
            var jobCardsYearly = await dB_LogisticsproContext.LpJobCard.Where(i => i.EffectiveDate.Value.Year== currentDate.Year).ToListAsync();

            dashboard.TotalJobCardYearly = jobCardsYearly.Count;

            var totaleTransfersYearly = 0;
            var totaleHotelYearly = 0;
            var totaleVisaYearly = 0;
            var totaleMiscYearly = 0;
            decimal totalJobCardValueYearly = 0;

            foreach (var jobCard in jobCardsYearly)
            {
                var trasnfers = await GetJobTransportations(jobCard.Id);
                totaleTransfersYearly = totaleTransfersYearly + (trasnfers == null ? 0 : trasnfers.Count);

                foreach (var trasnfer in trasnfers)
                {
                    totalJobCardValueYearly = totalJobCardValueYearly + trasnfer.TotalSellPrice;
                }

                var hotels = await GetJobHotels(jobCard.Id);
                totaleHotelYearly = totaleHotelYearly + (hotels == null ? 0 : hotels.Count);

                foreach (var hotel in hotels)
                {
                    totalJobCardValueYearly = totalJobCardValueYearly + hotel.TotalSellPrice;
                }

                var visas = await GetJobVisas(jobCard.Id);
                totaleVisaYearly = totaleVisaYearly + (visas == null ? 0 : visas.Count);

                foreach (var visa in visas)
                {
                    totalJobCardValueYearly = totalJobCardValueYearly + visa.TotalSellPrice;
                }

                var mises = await GetJobMiscellanea(jobCard.Id);
                totaleMiscYearly = totaleMiscYearly + (mises == null ? 0 : mises.Count);

                foreach (var mise in mises)
                {
                    totalJobCardValueYearly = totalJobCardValueYearly + mise.TotalSellPrice;
                }
            }

            dashboard.Chart_2.Yearly = new List<int?> { totaleTransfersYearly, totaleHotelYearly, totaleVisaYearly, totaleMiscYearly };
            dashboard.TotalJobCardValueYearly = totalJobCardValueYearly;


            var customerListYearly = jobCardsYearly.Where(i => i.CustomerId.HasValue).GroupBy(i => i.CustomerId).Select(i => new Customer { Id = i.Key.Value, TotalBookingCount = i.Count() }).ToList();
            List<Customer> customerYearly = new List<Customer>();
            foreach (var customer in customerListYearly)
            {
                var cus = await dB_LogisticsproContext.LpMCustomer.FirstOrDefaultAsync(i => i.Id == customer.Id);
                if (cus == null)
                {
                    continue;
                }

                customerYearly.Add(new Customer
                {
                    Id = customer.Id,
                    CustomerName = cus.CustomerName,
                    TotalBookingCount = customer.TotalBookingCount
                });
            }

            dashboard.CustomersYearly = customerYearly.OrderByDescending(i => i.TotalBookingCount).Take(10).ToList();

            var invoices = await dB_LogisticsproContext.LpFInvoice.Where(i => i.InvoiceDate >= yearStartDate && i.InvoiceDate <= yearEndDate && i.StatusId!=3).ToListAsync();
            dashboard.TotalInvoice = invoices.Count;

            decimal totalInvoiceValue = 0;
            var totalGeneratedInvoice = 0;
            var totalPaidInvoice = 0;
            var totalUnPaidInvoice = 0;

            foreach (var invoice in invoices)
            {
                totalInvoiceValue = totalInvoiceValue + (!invoice.InvoiceAmount.HasValue ? 0 : invoice.InvoiceAmount.Value);
            }

            var invoiceGenerated = invoices.Where(i => i.StatusId == 1).ToList();
            totalGeneratedInvoice = invoiceGenerated.Count;
            dashboard.TotalGeneratedInvoice= totalGeneratedInvoice;

            var invoiceUnPaid = invoices.Where(i => i.StatusId ==4).ToList();
            totalUnPaidInvoice = invoiceUnPaid.Count;
            dashboard.TotalUnPaidInvoice = totalUnPaidInvoice;

            var invoicePaid = invoices.Where(i => i.StatusId == 5).ToList();
            totalPaidInvoice = invoicePaid.Count;
            dashboard.TotalPaidInvoice = totalPaidInvoice;

            dashboard.TotalInvoiceValue = totalInvoiceValue;

            dashboard.Chart_1.Generated = GetInvoiceCountPerMonth(invoiceGenerated);
            dashboard.Chart_1.Paid = GetInvoiceCountPerMonth(invoicePaid);
            dashboard.Chart_1.UnPaid = GetInvoiceCountPerMonth(invoiceUnPaid);

            var paymentVoucherMonth = await dB_LogisticsproContext.LpFPaymentVoucher.Where(i => i.VoucherDate.Value.Month == currentDate.Month).ToListAsync();
            var pvListMonthly = paymentVoucherMonth.Where(i => i.VendorId.HasValue).GroupBy(i => i.VendorId).Select(i => new Vendor { Id = i.Key.Value, TotalBookingCount = i.Count() }).ToList();
            List<Vendor> vendorsMonthly = new List<Vendor>();
            foreach (var vendor in pvListMonthly)
            {
                var vender = await dB_LogisticsproContext.LpMVender.FirstOrDefaultAsync(i => i.Id == vendor.Id);
                if (vender == null)
                {
                    continue;
                }

                vendorsMonthly.Add(new Vendor
                {
                    Id = vendor.Id,
                    VendorName = vender.VenderName,
                    TotalBookingCount = vendor.TotalBookingCount
                });
            }

            dashboard.VendorsMonthly = vendorsMonthly.OrderByDescending(i => i.TotalBookingCount).Take(10).ToList();

            var paymentVoucherYear = await dB_LogisticsproContext.LpFPaymentVoucher.Where(i => i.VoucherDate.Value.Year == currentDate.Year).ToListAsync();
            var pvListYearly = paymentVoucherYear.Where(i => i.VendorId.HasValue).GroupBy(i => i.VendorId).Select(i => new Vendor { Id = i.Key.Value, TotalBookingCount = i.Count() }).ToList();
            List<Vendor> vendorsYearly = new List<Vendor>();
            foreach (var vendor in pvListYearly)
            {
                var vender = await dB_LogisticsproContext.LpMVender.FirstOrDefaultAsync(i => i.Id == vendor.Id);
                if (vender == null)
                {
                    continue;
                }

                vendorsYearly.Add(new Vendor
                {
                    Id = vendor.Id,
                    VendorName = vender.VenderName,
                    TotalBookingCount = vendor.TotalBookingCount
                });
            }

            dashboard.VendorsYearly = vendorsYearly.OrderByDescending(i=>i.TotalBookingCount).Take(10).ToList();
            return dashboard;
        }

        private List<int?> GetInvoiceCountPerMonth(List<LpFInvoice> invoices)
        {
            var currentDate = DateTime.UtcNow;

            var returnList=new List<int?>();
            int month = 1;

            while (currentDate.Month>=month)
            {
                var monthRecord = invoices.Where(i => i.InvoiceDate.Value.Date.Month == month).ToList();
                returnList.Add(monthRecord == null ? null : monthRecord.Count);
                month++;
            }
            return returnList;
        }

        public async Task<List<JobCardTransportation>> GetJobTransportations(int jobCardId)
        {
            var lpJobTransportation = await (from lpt in dB_LogisticsproContext.LpJTransportation
                                             where lpt.JobCardId == jobCardId
                                             select new JobCardTransportation
                                             {
                                                 Id = lpt.Id,
                                                 TransportationCode = lpt.TransportationCode,
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
                                                 Remarks = lpt.Remarks,
                                                 CostTaxAmount = lpt.CostTaxAmount,
                                                 SellTaxAmount = lpt.SellTaxAmount,
                                                 CostBaseAmount = lpt.CostBaseAmount,
                                                 SellBaseAmount = lpt.SellBaseAmount,
                                                 Parking = lpt.Parking,
                                                 ParkingTaxAmount = lpt.ParkingTaxAmount,
                                                 Water = lpt.Water,
                                                 WaterTaxAmount = lpt.WaterTaxAmount,
                                                 Extras = lpt.Extras,
                                                 ExtrasTaxAmount = lpt.ExtrasTaxAmount,
                                                 ParkingSell = lpt.ParkingSell,
                                                 ParkingTaxAmountSell= lpt.ParkingTaxAmountSell,
                                                 WaterSell = lpt.WaterSell,
                                                 WaterTaxAmountSell= lpt.WaterTaxAmountSell,
                                                 ExtrasSell = lpt.ExtrasSell,
                                                 ExtrasTaxAmountSell = lpt.ExtrasTaxAmountSell,
                                             }).OrderByDescending(i => i.Id).ToListAsync();


            return lpJobTransportation;
        }

        public async Task<List<JobCardHotel>> GetJobHotels(int jobCardId)
        {
            var lpJobHotels = await (from lph in dB_LogisticsproContext.LpJHotel
                                     join v in dB_LogisticsproContext.LpMVender on lph.VendorId equals v.Id
                                     where lph.JobCardId == jobCardId
                                     select new JobCardHotel
                                     {
                                         Id = lph.Id,
                                         HotelCode = lph.HotelCode,
                                         PaxName = lph.PaxName,
                                         CheckIn = lph.CheckIn,
                                         CheckOut = lph.CheckOut,
                                         Adults = lph.Adults,
                                         Children = lph.Children,
                                         Infants = lph.Infants,
                                         CostBaseAmount = lph.CostBaseAmount,
                                         CostTaxAmount = lph.CostTaxAmount,
                                         SellBaseAmount = lph.SellBaseAmount,
                                         SellTaxAmount = lph.SellTaxAmount,
                                         VenderName = v.VenderName,
                                         VenderId = v.Id,
                                         Remarks = lph.Remarks,
                                         RoomType = lph.RoomType,
                                         HotelConfirmation = lph.HotelConfirmation
                                     }).OrderByDescending(i => i.Id).ToListAsync();


            return lpJobHotels;
        }

        public async Task<List<JobCardVisa>> GetJobVisas(int jobCardId)
        {
            var lpJobVisa = await (from lpv in dB_LogisticsproContext.LpJVisa
                                   join lpvt in dB_LogisticsproContext.LpMVisaType on lpv.VisaTypeId equals lpvt.Id
                                   where lpv.JobCardId == jobCardId
                                   select new JobCardVisa
                                   {
                                       Id = lpv.Id,
                                       VisaCode = lpv.VisaCode,
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
                                   }).OrderByDescending(i => i.Id).ToListAsync();


            return lpJobVisa;
        }

        public async Task<List<JobCardMiscellaneous>> GetJobMiscellanea(int jobCardId)
        {
            var lpJobMiscellaneous = await (from lpm in dB_LogisticsproContext.LpJMiscellaneous
                                            where lpm.JobCardId == jobCardId
                                            select new JobCardMiscellaneous
                                            {
                                                Id = lpm.Id,
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


            return lpJobMiscellaneous;
        }
    }
}
