using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages
{
	public class IndexModel : PageModel
	{
		#region Constructors

		public IndexModel(ILoggerFactory loggerFactory)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		public virtual IDictionary<string, IList<string>> CertificateStores { get; } = new SortedDictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
		public virtual Exception Exception { get; set; }
		protected internal virtual ILogger Logger { get; }
		public virtual string SubjectFilter { get; set; }

		#endregion

		#region Methods

		public virtual void OnGet(string subjectFilter)
		{
			this.SubjectFilter = subjectFilter;

			if(string.IsNullOrWhiteSpace(subjectFilter))
				return;

			try
			{
				foreach(var storeLocation in Enum.GetValues<StoreLocation>())
				{
					foreach(var storeName in Enum.GetValues<StoreName>())
					{
						var certificates = new List<X509Certificate2>();

						try
						{
							using(var store = new X509Store(storeName, storeLocation))
							{
								store.Open(OpenFlags.ReadOnly);

								// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
								foreach(var certificate in store.Certificates)
								{
									var regexPattern = "^" + Regex.Escape(subjectFilter).Replace("\\*", ".*") + "$";

									if(Regex.IsMatch(certificate.Subject, regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
										certificates.Add(certificate);
								}
								// ReSharper restore ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
							}

							if(certificates.Any())
								this.CertificateStores.Add($"{storeLocation}\\{storeName}", certificates.Select(item => item.Subject).OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToList());
						}
						catch(Exception exception)
						{
							this.Logger.LogError(exception, "Error");
						}
					}
				}
			}
			catch(Exception exception)
			{
				this.Exception = exception;
			}
		}

		#endregion
	}
}