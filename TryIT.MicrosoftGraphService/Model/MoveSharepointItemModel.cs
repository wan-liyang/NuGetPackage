namespace TryIT.MicrosoftGraphService.Model
{
    /// <summary>
    /// model for move sharepoint item, e.g. move file into another folder
    /// </summary>
    public class MoveSharepointItemModel
    {
        /// <summary>
        /// source item id
        /// </summary>
        public string itemId { get; set; }
        /// <summary>
        /// destination folder id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// new name of item, leave empty if remain same name
        /// </summary>
        public string name { get; set; }
    }
}
