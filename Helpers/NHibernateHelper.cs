
using System.Configuration;
using System.Web;
using System.Web.Hosting;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using ivNet.Listing.Entities;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace ivNet.Listing.Helpers
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static string _dbFile;

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    SetupSessionFactory();
                return _sessionFactory;
            }
        }

        private static void SetupSessionFactory()
        {
            _sessionFactory = ConfigureSessionFactory();
        }

        private static ISessionFactory ConfigureSessionFactory()
        {
            var connectionString = GetConnectionString();

            return Fluently.Configure()
                .Database(MsSqlCeConfiguration.Standard.ShowSql().ConnectionString(c => c.Is(connectionString)))
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<Owner>()
                    .Conventions
                    .Add(Table.Is(x => "ivNet" + x.EntityType.Name), PrimaryKey.Name.Is(x => x.EntityType.Name + "ID"), ForeignKey.EndsWith("ID")))
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();
        }

        private static string GetConnectionString()
        {
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath("~/Modules/ivNet.Listing/web.config")
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var connectionString =
                configuration.ConnectionStrings.ConnectionStrings["ivNet.Listing.ConnectionString"].ConnectionString;

            _dbFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/Modules/ivNet.Listing/App_Data");

            return string.Format(@"Data Source={0}\{1}", _dbFile, connectionString);            
        }
    }
}