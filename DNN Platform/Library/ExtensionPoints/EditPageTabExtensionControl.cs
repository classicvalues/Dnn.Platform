﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.ExtensionPoints
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    [DefaultProperty("Module")]
    [ToolboxData("<{0}:EditPageTabExtensionControl runat=server></{0}:EditPageTabExtensionControl>")]
    public class EditPageTabExtensionControl : DefaultExtensionControl
    {
        [Bindable(true)]
        [DefaultValue("")]
        public string TabControlId
        {
            get
            {
                var s = (string)this.ViewState["TabControlId"];
                return s ?? string.Empty;
            }

            set
            {
                this.ViewState["TabControlId"] = value;
            }
        }

        [Bindable(true)]
        [DefaultValue("")]
        public string PanelControlId
        {
            get
            {
                var s = (string)this.ViewState["PanelControlId"];
                return s ?? string.Empty;
            }

            set
            {
                this.ViewState["PanelControlId"] = value;
            }
        }

        public void BindAction(int portalId, int tabId, int moduleId)
        {
            var panel = this.Parent.FindControl(this.PanelControlId);

            foreach (var control in panel.Controls)
            {
                var panelcontrol = control as PanelTabExtensionControl;
                if (panelcontrol != null)
                {
                    foreach (var extensionControl in panelcontrol.Controls)
                    {
                        var actionsControl = extensionControl as IEditPageTabControlActions;
                        if (actionsControl != null)
                        {
                            actionsControl.BindAction(portalId, tabId, moduleId);
                        }
                    }
                }
            }
        }

        public void SaveAction(int portalId, int tabId, int moduleId)
        {
            var panel = this.Parent.FindControl(this.PanelControlId);

            foreach (var control in panel.Controls)
            {
                var panelcontrol = control as PanelTabExtensionControl;
                if (panelcontrol != null)
                {
                    foreach (var extensionControl in panelcontrol.Controls)
                    {
                        var actionsControl = extensionControl as IEditPageTabControlActions;
                        if (actionsControl != null)
                        {
                            actionsControl.SaveAction(portalId, tabId, moduleId);
                        }
                    }
                }
            }
        }

        public void CancelAction(int portalId, int tabId, int moduleId)
        {
            var panel = this.Parent.FindControl(this.PanelControlId);

            foreach (var control in panel.Controls)
            {
                var panelcontrol = control as PanelTabExtensionControl;
                if (panelcontrol != null)
                {
                    foreach (var extensionControl in panelcontrol.Controls)
                    {
                        var actionsControl = extensionControl as IEditPageTabControlActions;
                        if (actionsControl != null)
                        {
                            actionsControl.CancelAction(portalId, tabId, moduleId);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnInit(EventArgs e)
        {
            var extensionPointManager = new ExtensionPointManager();

            var tabs = (HtmlGenericControl)this.Parent.FindControl(this.TabControlId);
            var panel = this.Parent.FindControl(this.PanelControlId);

            foreach (var extension in extensionPointManager.GetEditPageTabExtensionPoints(this.Module, this.Group))
            {
                if (extension.Visible)
                {
                    var liElement = new HtmlGenericControl("li")
                    {
                        InnerHtml = "<a href=\"#" + extension.EditPageTabId + "\">" + extension.Text + "</a>",
                    };
                    liElement.Attributes.Add("class", extension.CssClass);
                    tabs.Controls.Add(liElement);

                    var container = new PanelTabExtensionControl { PanelId = extension.EditPageTabId };
                    var control = this.Page.LoadControl(extension.UserControlSrc);
                    control.ID = Path.GetFileNameWithoutExtension(extension.UserControlSrc);
                    container.Controls.Add(control);
                    panel.Controls.Add(container);
                }
            }
        }
    }
}
