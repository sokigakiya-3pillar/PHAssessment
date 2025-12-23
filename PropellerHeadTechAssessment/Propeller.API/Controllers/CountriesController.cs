using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Propeller.API.Resources;
using Propeller.DALC.Interfaces;
using Propeller.Models;

namespace Propeller.API.Controllers
{

    [Route("api/countries")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IStringLocalizer<GeneralResources> _localizer;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerStatusController> _logger;

        public CountriesController(IStringLocalizer<GeneralResources> localizer,
            IMapper mapper,
            ICountriesRepository countriesRepository,
            ILogger<CustomerStatusController> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _countriesRepository = countriesRepository ?? throw new ArgumentNullException(nameof(countriesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        // [Authorize(Policy = "IsAdminUser")]
        [Authorize(Roles = "Admin,Power", Policy = "IsNZLUser")]
        public async Task<ActionResult<CountryDto>> RetrieveCountries()
        {
            try
            {
                var result = await _countriesRepository.RetrieveCountries();

                List<CountryDto> countries = new List<CountryDto>();

                // _localizer.GetString("", )

                foreach (var item in result)
                {
                    countries.Add(
                        new CountryDto
                        {
                            ID = item.ID,
                            Name = _localizer[item.CountryCode],
                            CountryCode = item.CountryCode,
                            DefaultLocale = item.DefaultLocale
                        }
                        );
                }

                return Ok(countries);

                // return Ok(_mapper.Map<IEnumerable<CustomerStatusDto>>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception ocurred when Retrieving Customer Statuses");
                return StatusCode(500, "Unable to retrieve Statuses");
            }

        }

    }
}
