// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Specification;
using GUI.BoxArrowDiagram;
using GUI.LongOperations;
using GUI.Properties;
using ModelElement = Utils.ModelElement;

namespace GUI.RequirementSetDiagram
{
    public class RequirementSetControl : BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RequirementSetControl()
        {
            MouseDoubleClick += HandleMouseDoubleClick;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public RequirementSetControl(IContainer container)
            : base(container)
        {
            MouseDoubleClick += HandleMouseDoubleClick;
        }

        public override void AcceptDrop(ModelElement element)
        {
            base.AcceptDrop(element);

            // Allows to allocate paragraphs in requirement sets
            Paragraph paragraph = element as Paragraph;
            if (paragraph != null)
            {
                if (!paragraph.AppendToRequirementSet(Model))
                {
                    MessageBox.Show(
                        Resources.RequirementSetControl_AcceptDrop_Paragraph_not_added_to_the_requirement_set_because_it_already_belongs_to_it,
                        Resources.RequirementSetControl_AcceptDrop_Paragraph_not_added, 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                }
            }
        }
        
        /// <summary>
        ///     Handles a double click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            RequirementSetPanel panel = (RequirementSetPanel) Panel;
            if (panel != null)
            {
                RequirementSetDiagramWindow window = new RequirementSetDiagramWindow();
                GuiUtils.MdiWindow.AddChildWindow(window);
                window.Text = Model.Name;
                OpenRequirementSetOperation openRequirementSet = new OpenRequirementSetOperation(window, Model);
                openRequirementSet.ExecuteUsingProgressDialog("Opening requirement set " + Name);
            }
        }

        /// <summary>
        ///     Implemented color
        /// </summary>
        public static Color ImplementedColor = Color.Green;

        public static Pen ImplementedPen = new Pen(ImplementedColor);

        /// <summary>
        ///     Tested color
        /// </summary>
        public static Color TestedColor = Color.Yellow;

        public static Pen TestedPen = new Pen(TestedColor);

        /// <summary>
        ///     Draws the box within the box-arrow panel
        /// </summary>
        /// <param name="g"></param>
        public override void PaintInBoxArrowPanel(Graphics g)
        {
            SetColor(Color.Transparent);

            // Draws the enclosing box
            g.FillRectangle(new SolidBrush(NormalColor), Location.X, Location.Y, Width, Height);
            g.DrawRectangle(NormalPen, Location.X, Location.Y, Width, Height);

            // Draws the completion box
            g.DrawRectangle(NormalPen, Location.X + 10, Location.Y + Height - 20, Width - 20, 10);
            Paragraph.ParagraphSetMetrics metrics;
            if (((RequirementSetPanel) Panel).Metrics.TryGetValue(Model, out metrics))
            {
                FillCompletion(g, metrics.ImplementedCount, metrics.ImplementableCount, ImplementedColor, 10);
                FillCompletion(g, metrics.TestedCount, metrics.ImplementableCount, TestedColor, 5);                
            }
        }

        /// <summary>
        ///     Fills the progression bar according to the task ratio completed
        /// </summary>
        /// <param name="g"></param>
        /// <param name="performed"></param>
        /// <param name="total"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        private void FillCompletion(Graphics g, int performed, int total, Color color, int width)
        {
            double ratio = 1;
            if (total > 0)
            {
                // ReSharper disable RedundantCast
                ratio = (double) performed/(double) total;
                // ReSharper restore RedundantCast
            }
            g.FillRectangle(new SolidBrush(color), Location.X + 10 + 1, Location.Y + Height - 20 + 10 - width + 1,
                (int) ((Width - 20)*ratio) - 1, width - 1);
        }
    }
}