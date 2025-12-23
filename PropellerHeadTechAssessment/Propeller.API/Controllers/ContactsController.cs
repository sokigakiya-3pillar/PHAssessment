using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Propeller.DALC.Interfaces;
using Propeller.Entities;
using Propeller.Models;
using Propeller.Models.Requests;
using Propeller.Shared;
using System.Text.Json;

namespace Propeller.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;

        private readonly IContactsRepository _contactsRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IMapper _mapper;

        private int maxPageSize = 50;
        private int minPageSize = 5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactsRepo"></param>
        /// <param name="customerRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ContactsController(IContactsRepository contactsRepo, ICustomerRepository customerRepository,
            IMapper mapper, ILogger<CustomersController> logger
        )
        {
            _contactsRepo = contactsRepo ?? throw new ArgumentNullException(nameof(contactsRepo));
            _customerRepo = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a New Contact
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Power")]
        public async Task<ActionResult> CreateContact(CreateContactRequest request)
        {
            try
            {
                Contact newContact = _mapper.Map<Contact>(request);

                if (string.IsNullOrEmpty(newContact.EMail.Trim()) && 
                    string.IsNullOrEmpty(newContact.PhoneNumber.Trim()))
                {
                    return UnprocessableEntity("Email or Phone Required");
                }

                // The contact might have been added before for another costumer, if so
                // then attach the existing Contact to the Customer
                var existingUser = await _contactsRepo.RetrieveContact(newContact);

                if (existingUser != null)
                {
                    return Ok(existingUser);
                }

                // A contact can be associated or not to a Customer
                if (!string.IsNullOrEmpty(request.CustomerID))
                {

                    int cid = request.CustomerID.Deobfuscate();

                    if (cid == -1)
                    {
                        return BadRequest(); // TODO: SHould it be Bad request?
                    }

                    var customer = await _customerRepo.RetrieveCustomerAsync(request.CustomerID.Deobfuscate());

                    if (customer == null)
                    {
                        return NotFound("Customer Not Found");
                    }

                    newContact.Customers.Add(customer);

                }

                var contact = await _contactsRepo.InsertContactAsync(newContact);

                // Validate Customr exists
                //var existingCustomer = await _ _customerRepo.RetrieveCustomerAsync(customerId);

                //if (existingCustomer == null)
                //{
                //    return NotFound();
                //}

                //Note note = new Note
                //{
                //    CustomerID = customerId,
                //    Text = noteText,
                //    TimeStamp = DateTime.UtcNow
                //};

                //await _notesRepository.InsertNoteAsync(note);
                // return Ok();

                // var newCustomer = _mapper.Map<Customer>(request);
                // var result = await _customerRepo.InsertCustomerAsync(newCustomer);
                // return Ok(_mapper.Map<ContactDto>(r));
                return CreatedAtRoute(
                        "GetContact",
                        new { ctid = contact.ID },
                        _mapper.Map<ContactDto>(contact)
                );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception ocurred when Adding new Contact");
                return StatusCode(500, "Unable to Add Contact");
            }

        }

        /// <summary>
        /// Retrieves a contact by ID
        /// </summary>
        /// <param name="ctid">The Contact's ID</param>
        /// <returns></returns>
        [HttpGet("{ctid}", Name = "GetContact")]
        public async Task<ActionResult<ContactDto>> RetrieveContact(int ctid)
        {
            try
            {
                var existingContact = await _contactsRepo.RetrieveContact(ctid);

                if (existingContact == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<ContactDto>(existingContact));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception ocurred when Retrieving Contact. CID:{ctid}");
                return StatusCode(500, "Unable to Retrieve Contact");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin,Power")]
        public async Task<ActionResult<bool>> UpdateContact(int id, UpdateContactRequest request)
        {
            try
            {
                // TODO: Add validation for email and phone

                var existingContact = await _contactsRepo.RetrieveContact(id);

                if (existingContact == null)
                {
                    return NotFound();
                }

                _mapper.Map(request, existingContact);

                await _customerRepo.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception ocurred when Updating Contact. CID:{id}");
                return StatusCode(500, "Unable to update Contact");
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<List<Contact>>> RetrieveContacts(int customerID)
        //{
        //    try
        //    {
        //        var contacts = await _contactsRepo.RetrieveContacts(customerID);
        //        return Ok(_mapper.Map<IEnumerable<ContactDto>>(contacts));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Exception ocurred when Retrieving Contacts. CID:{customerID}");
        //        return StatusCode(500, "Unable to retrieve Contacts");
        //    }
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> RetrieveContacts(
                [FromQuery(Name = "q")] string? criteria,
                [FromQuery(Name = "pn")] int pageNumber = 1,
                [FromQuery(Name = "ps")] int pageSize = 50,
                [FromQuery(Name = "sf")] string? searchField = ""
            )
        {

            try
            {
                if (pageSize < minPageSize) { pageSize = minPageSize; }
                else if (pageSize > maxPageSize) { pageSize = maxPageSize; }

                if (string.IsNullOrEmpty(criteria))
                {
                    criteria = string.Empty;
                }

                if (string.IsNullOrEmpty(searchField))
                {
                    searchField = string.Empty;
                }

                var result = await _contactsRepo.RetrieveContactsAsync(criteria.Trim(), searchField.Trim(), pageNumber, pageSize);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.pagination));

                return Ok(_mapper.Map<IEnumerable<ContactDto>>(result.contacts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception ocurred when Retrieving Contacts");
                return StatusCode(500, "Unable to retrieve Contacts");
            }
        }

        /// <summary>
        /// Contacts can be associated to one or more customers, if so, we should not delete them
        /// unless we forcefully want to delete it (flag)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteContact(
            [FromRoute(Name = "id")] int contactId,
            [FromQuery(Name = "fd")] string? forceDelete = "n")
        {

            try
            {
                var contact = await _contactsRepo.RetrieveContact(contactId);

                if (contact == null)
                {
                    return NotFound();
                }

                if (contact.Customers.Any() && forceDelete != "y")
                {
                    return Forbid();
                }

                // Remove
                contact.Customers.Clear();
                int recordsAffected = await _contactsRepo.DeleteContactAsync(contact);

                if (recordsAffected == 0) // No data was deleted, no error
                {
                    return Ok();                }

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception ocurred when Retrieving Contacts");
                return StatusCode(500, "Unable to retrieve Contacts");
            }

        }

    }
}
