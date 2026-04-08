namespace Models{

    public class View
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public int? UserId { get; set; } // može i guest view
        public User User { get; set; }

        public DateTime ViewedAt { get; set; }
    }
}