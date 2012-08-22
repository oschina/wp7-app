
namespace OSChina. Model
{
    public class BaseUnit
    {
        public int id { get; set; }
    }

    public class BaseUnitWithType : BaseUnit
    {
        public int type { get; set; }

        public virtual string Key( )
        {
            return string. Format( "{0}_{1}", this. id. ToString( ), this. type. ToString( ) );
        }
    }

    public class BaseUnitWithName
    {
        public string name { get; set; }
    }
}
