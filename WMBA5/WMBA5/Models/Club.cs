namespace WMBA5.Models
{
    public class Club
    {
        public int ID { get; set; }
        public string ClubName { get; set; }
        public ICollection<Division> Divisions { get; set; } = new HashSet<Division>();
    }
}
