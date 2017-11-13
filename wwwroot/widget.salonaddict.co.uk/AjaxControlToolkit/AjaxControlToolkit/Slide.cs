namespace AjaxControlToolkit
{
    using System;

    [Serializable]
    public class Slide
    {
        private string description;
        private string imagePath;
        private string name;

        public Slide() : this(null, null, null)
        {
        }

        public Slide(string imagePath, string name, string description)
        {
            this.imagePath = imagePath;
            this.name = name;
            this.description = description;
        }

        public string Description
        {
            get => 
                this.description;
            set
            {
                this.description = value;
            }
        }

        public string ImagePath
        {
            get => 
                this.imagePath;
            set
            {
                this.imagePath = value;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

