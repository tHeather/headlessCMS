namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryJoin
    {
        public string LeftCollectionName { get; set; }

        public string RightCollectionName { get; set; }

        public string LeftOnField { get; set; }

        public string RightOnField { get; set; }

        public string Type { get; set; }
    }
}
