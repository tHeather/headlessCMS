using headlessCMS.Enums;

namespace headlessCMS.Models.Services
{
    public class DeleteQueryParametersDataCollection
    {
        public string CollectionName { get; set; }

        public List<IdAndDataState> IdsAndDataStates { get; set; }
    }
}
