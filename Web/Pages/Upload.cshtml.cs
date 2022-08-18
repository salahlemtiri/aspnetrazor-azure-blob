using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Web.Helpers;

namespace Web.Pages
{
	public class UploadModel : PageModel
	{

		BlobContainerClient _containerClient;


		public UploadModel(IOptions<AzureStorageConfig> config, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				_containerClient = new BlobContainerClient(config.Value.AccountName, config.Value.ContainerName);
			}
			else
			{
				string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
														config.Value.AccountName,
													   config.Value.ContainerName);

				_containerClient = new BlobContainerClient(new Uri(containerEndpoint),
																			new DefaultAzureCredential());
			}
		}

		[BindProperty]
		public IFormFile Upload { get; set; }




		public async Task<IActionResult> OnPostAsync()
		{

			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				// Create the container if it does not exist.
				await _containerClient.CreateIfNotExistsAsync();

				// Upload the file to the container
				await _containerClient.UploadBlobAsync(Upload.FileName, Upload.OpenReadStream());
			}
			catch (Exception e)
			{
				throw e;
			}



			return Page();

		}
	}
}
