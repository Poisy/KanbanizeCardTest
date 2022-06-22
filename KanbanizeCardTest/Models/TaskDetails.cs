namespace KanbanizeCardTest.Models
{
    public class TaskDetails
    {
        public string Taskid { get; set; }
        public string Boardid { get; set; }
        public string Title { get; set; }
        public object Description { get; set; }
        public string Type { get; set; }
        public string Assignee { get; set; }
        public string Subtasks { get; set; }
        public string Subtaskscomplete { get; set; }
        public string Color { get; set; }
        public string Priority { get; set; }
        public object Size { get; set; }
        public object Deadline { get; set; }
        public object Deadlineoriginalformat { get; set; }
        public string Extlink { get; set; }
        public string Tags { get; set; }
        public int Leadtime { get; set; }
        public string Blocked { get; set; }
        public object Blockedreason { get; set; }
        public string Columnname { get; set; }
        public string Lanename { get; set; }
        public object[] Subtaskdetails { get; set; }
        public string Columnid { get; set; }
        public string Laneid { get; set; }
        public string Position { get; set; }
        public int Workflow { get; set; }
        public int WorkflowId { get; set; }
        public object Attachments { get; set; }
        public string Columnpath { get; set; }
        public object[] Customfields { get; set; }
        public string Updatedat { get; set; }
        public int Loggedtime { get; set; }
        public string Reporter { get; set; }
    }
}