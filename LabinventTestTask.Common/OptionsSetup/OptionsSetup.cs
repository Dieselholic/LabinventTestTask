using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LabinventTestTask.Common.OptionsSetup
{
    public class OptionsSetup<T> : IConfigureNamedOptions<T> where T : class
    {
        private readonly string _sectionName;
        private readonly IConfiguration _configuration;

        public OptionsSetup(IConfiguration configuration)
        {
            _sectionName = typeof(T).Name;
            _configuration = configuration;
        }

        public void Configure(T options)
        {
            _configuration.GetSection(_sectionName).Bind(options);
        }

        public void Configure(string? name, T options) => Configure(options);
    }
}
