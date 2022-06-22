namespace KanbanizeCardTest.Models
{
    public class Project
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Board[] Boards { get; set; }
    }
}