using Exortech.NetReflector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ccnet.datetimelabeller.plugin
{
    [ReflectorType("datetimeLabeller")]
    public class DatetimeLabeller : ITask, ILabeller
    {
        public DatetimeLabeller()
        {
        }

        #region ITask Members

        public void Run(IIntegrationResult result)
        {
            result.Label = this.Generate(result);
        }

        #endregion ITask Members

        #region ILabeller Members

        public string Generate(IIntegrationResult integrationResult)
        {
            string version = "0.0.0.1";
            if (string.IsNullOrEmpty(_build))
            {
                _build = GetBuild(this.DateTimeFormat);
            }
            var _rebuild = GetRebuildNumber(_build, integrationResult.LastSuccessfulIntegrationLabel);
            version = string.Format("{0}{1}.{2}.{3}.{4}", Prefix, Major, Minor, Build, _rebuild);
            return version;
        }

        private int GetRebuildNumber(string currentBuild, string lastLabel)
        {
            var p = @"([\d]+)\.([\d]+)\.([\d]+)\.([\d+])";
            var v = Regex.Match(lastLabel, p).Groups[3].Value;
            var r = Regex.Match(lastLabel, p).Groups[4].Value;

            int rebuild = 0;
            Int32.TryParse(r, out rebuild);

            if (v == currentBuild)
            {
                rebuild++;
            }
            else
            {
                rebuild = 1;
            }
            return rebuild;
        }

        private string GetBuild(string datetimeFormat)
        {
            return DateTime.Now.ToString(datetimeFormat);
        }

        #endregion ILabeller Members

        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        /// <value>The major version number.</value>
        private int _major = 1;

        [ReflectorProperty("major", Required = false)]
        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        /// <value>The minor version number.</value>
        private int _minor = 0;

        [ReflectorProperty("minor", Required = false)]
        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        /// <value>The build number.</value>
        private string _build = "";

        [ReflectorProperty("build", Required = false)]
        public string Build
        {
            get { return _build; }
            set { _build = value; }
        }

        /// <summary>
        /// Gets or sets the datetime format in build.
        /// </summary>
        /// <value>The build format.</value>
        private string _format = "yyyyMMdd";

        [ReflectorProperty("datetimeFormat", Required = false)]
        public string DateTimeFormat
        {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// Prefix for the build label.
        /// </summary>
        [ReflectorProperty("prefix", Required = false)]
        public string Prefix { get; set; }
    }
}