using System;

namespace PCL
{
    public class ExportOption
    {
        public ExportOption()
        {
            this.RequireModLoader = false;
            this.RequireOptiFine = false;
            this.RequireModLoaderOrOptiFine = false;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Rules { get; set; }

        public string ShowRules { get; set; }

        public bool DefaultChecked { get; set; }

        public bool RequireModLoader { get; set; }

        public bool RequireOptiFine { get; set; }

        public bool RequireModLoaderOrOptiFine { get; set; }
    }
}
