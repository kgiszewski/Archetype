using System;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace DatabaseSizer
{
    public class Application : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            ExpandPrevalueValueColumnSize();
        }

        protected void ExpandPrevalueValueColumnSize()
        {
            try
            {
                var dbContext = ApplicationContext.Current.DatabaseContext;
                var colCount =
                    dbContext.Database.ExecuteScalar<int>(
                        "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE DATA_TYPE = 'nvarchar' AND COLUMN_NAME = 'value' AND CHARACTER_MAXIMUM_LENGTH = 2500 AND TABLE_NAME = 'cmsDataTypePreValues'");

                // Check column
                if (colCount != 0)
                {
                    using (var trans = dbContext.Database.GetTransaction())
                    {
                        // Change column
                        dbContext.Database.Execute("ALTER TABLE [cmsDataTypePreValues] ALTER COLUMN [value] ntext NULL;");

                        trans.Complete();
                    }

                    LogHelper.Debug(typeof(Application), "Successfully expanded Prevalue table Value column");
                }
                else
                {
                    LogHelper.Debug(typeof(Application), "Prevalue table Value column is already expanded");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(Application), "Error expanding Prevalue table Value column", ex);
            }
        }
    }
}