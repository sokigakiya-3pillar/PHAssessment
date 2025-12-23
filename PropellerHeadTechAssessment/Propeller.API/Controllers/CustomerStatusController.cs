using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Propeller.DALC.Interfaces;
using Propeller.Entities;
using Propeller.Models;

namespace Propeller.API.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("api/status")]
    public class CustomerStatusController : ControllerBase
    {
        private ICustomerStatusRepository _customerStatusRepository;
        private readonly ILogger<CustomerStatusController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactsRepo"></param>
        /// <param name="customerStatusRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CustomerStatusController(IContactsRepository contactsRepo,
                ICustomerStatusRepository customerStatusRepository,
                IMapper mapper,
                ILogger<CustomerStatusController> logger
        )
        {
            _customerStatusRepository = customerStatusRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerStatus>>> RetrieveCustomerStatuses()
        {
            try
            {
                var result = await _customerStatusRepository.RetrieveStatusesAsync(); 
                return Ok(_mapper.Map<IEnumerable<CustomerStatusDto>>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception ocurred when Retrieving Customer Statuses");
                return StatusCode(500, "Unable to retrieve Statuses");
            }
        }

    }
}
