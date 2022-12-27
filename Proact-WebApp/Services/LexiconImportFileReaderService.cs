using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp {

    public static class LexiconImportFileReaderServiceExtensions {
        public static void AddLexiconImportFileReaderService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<ILexiconImportFileReaderService, LexiconImportFileReaderService>();
        }
    }

    public class LexiconImportFileReaderService : BaseService, ILexiconImportFileReaderService {

        private List<LexiconCreateCategoryRequest> _categories;

        public LexiconImportFileReaderService(
           ITokenAcquisition tokenAcquisition,
           HttpClient httpClient,
           IConfiguration configuration,
           IHttpContextAccessor contextAccessor )
           : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }



        public List<LexiconCreateCategoryRequest> ImportCategories( Stream fileStream ) {

            _categories = new List<LexiconCreateCategoryRequest>();

            System.Text.Encoding.RegisterProvider( System.Text.CodePagesEncodingProvider.Instance );
            using ( var reader = ExcelReaderFactory.CreateReader( fileStream ) ) {
        
                var dataSet = reader.AsDataSet();
                var dataRows = dataSet.Tables[0].Rows;
                dataRows.RemoveAt( 0 );
                foreach(DataRow row in dataRows ) {
                    InsertCategory( row );
                    InsertItem( row );
                }
            } 

            return _categories;
        }

        public bool ValidateFileExtention( IFormFile file ) {
            return true;
        }

        private void InsertCategory( DataRow dataRow ) {
            if ( dataRow.ItemArray.Length >= 4 ) {
                var category = dataRow.ItemArray[0].ToString();
                var multipleSelection = false;
                bool.TryParse( dataRow.ItemArray[1].ToString(), out multipleSelection);

                if ( !string.IsNullOrEmpty( ( ( string )category ) ) ) {

                    if ( !ContainCategory( category ) ) {
                        _categories.Add( new LexiconCreateCategoryRequest() {
                            Name = category,
                            MultipleSelection = multipleSelection,
                            Labels = new List<LexiconCreateLabelRequest>()
                        } );
                    }
                }
            }
        }

        private void InsertItem( DataRow dataRow ) {
            if ( dataRow.ItemArray.Length >= 4 ) {
                var category = dataRow.ItemArray[0].ToString();
                var item = dataRow.ItemArray[2].ToString();
                var group = dataRow.ItemArray[3].ToString();

               var categoryModel = _categories
                    .FirstOrDefault( x => x.Name == category );

                if ( !ContainItem( categoryModel, item ) ) {
                    categoryModel.Labels.Add( new LexiconCreateLabelRequest() {
                        Label = item,
                        GroupName = group
                    } );
                }
            }
        }

        private bool ContainCategory( string category ) {
            return _categories
                .FirstOrDefault( x => x.Name == category ) != null;
        }

        private bool ContainItem( LexiconCreateCategoryRequest category, string itemLlabel ) {
            return category.Labels
              .FirstOrDefault( x => x.Label == itemLlabel ) != null;
        }
         
    }
}
