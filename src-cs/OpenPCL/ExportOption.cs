using System;
using System.Runtime.CompilerServices;

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

        public string Title
        {
            [CompilerGenerated]
            get
            {
                return this._Title;
            }
            [CompilerGenerated]
            set
            {
                this._Title = value;
            }
        }

        public string Description
        {
            [CompilerGenerated]
            get
            {
                return this._Description;
            }
            [CompilerGenerated]
            set
            {
                this._Description = value;
            }
        }

        public string Rules
        {
            [CompilerGenerated]
            get
            {
                return this._Rules;
            }
            [CompilerGenerated]
            set
            {
                this._Rules = value;
            }
        }

        public string ShowRules
        {
            [CompilerGenerated]
            get
            {
                return this._ShowRules;
            }
            [CompilerGenerated]
            set
            {
                this._ShowRules = value;
            }
        }

        public bool DefaultChecked
        {
            [CompilerGenerated]
            get
            {
                return this._DefaultChecked;
            }
            [CompilerGenerated]
            set
            {
                this._DefaultChecked = value;
            }
        }

        public bool RequireModLoader
        {
            [CompilerGenerated]
            get
            {
                return this._RequireModLoader;
            }
            [CompilerGenerated]
            set
            {
                this._RequireModLoader = value;
            }
        }

        public bool RequireOptiFine
        {
            [CompilerGenerated]
            get
            {
                return this._RequireOptiFine;
            }
            [CompilerGenerated]
            set
            {
                this._RequireOptiFine = value;
            }
        }

        public bool RequireModLoaderOrOptiFine
        {
            [CompilerGenerated]
            get
            {
                return this._RequireModLoaderOrOptiFine;
            }
            [CompilerGenerated]
            set
            {
                this._RequireModLoaderOrOptiFine = value;
            }
        }

        [CompilerGenerated]
        private string _Title;

        [CompilerGenerated]
        private string _Description;

        [CompilerGenerated]
        private string _Rules;

        [CompilerGenerated]
        private string _ShowRules;

        [CompilerGenerated]
        private bool _DefaultChecked;

        [CompilerGenerated]
        private bool _RequireModLoader;

        [CompilerGenerated]
        private bool _RequireOptiFine;

        [CompilerGenerated]
        private bool _RequireModLoaderOrOptiFine;
    }
}
