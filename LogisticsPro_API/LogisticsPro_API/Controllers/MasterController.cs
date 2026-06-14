using LogisticsPro_API.Request;
using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Repository;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPro_API.Controllers
{
    // Routes follow BA Section 8.6 (resource-oriented) and 8.5 (HTTP method usage):
    // GET = retrieval, POST = create, PUT = update, DELETE = removal.
    [ApiController]
    public class MasterController : BaseController
    {
        private readonly IMediator mediator;
        public MasterController(IMediator mediator, JwtService jwtservice) : base(jwtservice)
        {
            this.mediator = mediator;
        }

        [HttpGet("~/api/reference-data")]
        [Authorize]
        public async Task<JsonResult> GetReferanceData()
        {
            var userId = GetUserIdFromToken();
            var referances = await mediator.Send(new ReferenceEventQuery((int)userId));
            return new JsonResult(referances);
        }

        [HttpGet("~/api/jobcards/reference-data")]
        [Authorize]
        public async Task<JsonResult> GetJobCardReferanceData(int customerId)
        {
            var userId = GetUserIdFromToken();
            var referances = await mediator.Send(new JobCardReferenceEventQuery((int)userId, customerId));
            return new JsonResult(referances);
        }

        // ----- Customers -----

        [HttpGet("~/api/customers")]
        [Authorize]
        public async Task<JsonResult> GetCustomers()
        {
            var userId = GetUserIdFromToken();
            var customers = await mediator.Send(new CustomerEventQuery((int)userId));
            return new JsonResult(customers);
        }

        [HttpPost("~/api/customers")]
        [HttpPut("~/api/customers")]
        [Authorize]
        public async Task<JsonResult> SaveCustomer(SaveCustomerRequest saveCustomerRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveCustomerCommand(saveCustomerRequest.Id, saveCustomerRequest.CustomerName, saveCustomerRequest.ContactPersonName, saveCustomerRequest.Email, saveCustomerRequest.ContactNumber, saveCustomerRequest.Trn, saveCustomerRequest.Address1, saveCustomerRequest.Address2, saveCustomerRequest.City, saveCustomerRequest.CountryCode, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/customers/{id:int}")]
        [Authorize]
        public async Task<JsonResult> RemoveCustomer(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveCustomerCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Vendors -----

        [HttpGet("~/api/vendors")]
        [Authorize]
        public async Task<JsonResult> GetVendors()
        {
            var userId = GetUserIdFromToken();
            var customers = await mediator.Send(new VendorEventQuery((int)userId));
            return new JsonResult(customers);
        }

        [HttpPost("~/api/vendors")]
        [HttpPut("~/api/vendors")]
        [Authorize]
        public async Task<JsonResult> SaveVendor(SaveVendorRequest saveVendorRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveVendorCommand(saveVendorRequest.Id, saveVendorRequest.VendorName, saveVendorRequest.ContactPersonName, saveVendorRequest.Email, saveVendorRequest.ContactNumber, saveVendorRequest.Trn, saveVendorRequest.Address1, saveVendorRequest.Address2, saveVendorRequest.City, saveVendorRequest.CountryCode, saveVendorRequest.BankCode, saveVendorRequest.Iban, saveVendorRequest.SwiftCode, saveVendorRequest.BankName, saveVendorRequest.BankBranch,saveVendorRequest.SelectedVendorTypes, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/vendors/{id:int}")]
        [Authorize]
        public async Task<JsonResult> RemoveVendor(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveVendorCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Users & Authentication -----

        [HttpGet("~/api/users")]
        [Authorize]
        public async Task<JsonResult> GetUsers()
        {
            var userId = GetUserIdFromToken();
            var users = await mediator.Send(new UserEventQuery((int)userId));
            return new JsonResult(users);
        }

        [HttpPost("~/api/users")]
        [HttpPut("~/api/users")]
        [Authorize]
        public async Task<JsonResult> SaveUser(SaveUserRequest saveUserRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveUserCommand(saveUserRequest.Id, saveUserRequest.UserName, saveUserRequest.Password, saveUserRequest.RoleId, saveUserRequest.StatusId, (int)userId));

            return new JsonResult(result);
        }

        // BA 8.9: authentication endpoint.
        [HttpPost("~/api/auth/login")]
        public async Task<JsonResult> LoginUser(LoginUserRequest loginUserRequest)
        {
            if (string.IsNullOrEmpty(loginUserRequest.UserName) || string.IsNullOrEmpty(loginUserRequest.Password))
            {
                var errorMessage = "Invalid Request";
                Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                return new JsonResult(new RequestSaveEnvelop(false, string.Empty, error));
            }

            var result = await mediator.Send(new LoginUserCommand(loginUserRequest.UserName, loginUserRequest.Password));

            return new JsonResult(result);
        }

        // ----- Events -----

        [HttpGet("~/api/events")]
        [Authorize]
        public async Task<JsonResult> GetEvents()
        {
            var userId = GetUserIdFromToken();
            var customers = await mediator.Send(new EventEventQuery((int)userId));
            return new JsonResult(customers);
        }

        [HttpPost("~/api/events")]
        [HttpPut("~/api/events")]
        [Authorize]
        public async Task<JsonResult> SaveEvent(SaveEventRequest saveEventRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveEventCommand(saveEventRequest.Id, saveEventRequest.EventName, DateTime.Parse(saveEventRequest.EventFromDate), DateTime.Parse(saveEventRequest.EventToDate), saveEventRequest.EventTypeId, saveEventRequest.CustomerId, saveEventRequest.Remark, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/events/{id:int}")]
        [Authorize]
        public async Task<JsonResult> RemoveEvent(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveEventCommand(id, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/dashboard")]
        [Authorize]
        public async Task<JsonResult> GetDashboardItems()
        {
            var userId = GetUserIdFromToken();
            var dashboard = await mediator.Send(new DashboardEventQuery((int)userId));
            return new JsonResult(dashboard);
        }
    }
}