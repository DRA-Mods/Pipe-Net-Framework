using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PipeNetFramework.Gizmos
{
    public class Command_ActionMulti : Command
    {
        public Action<int> action;
        public Func<int, string> floatMenuLabels;
        public int actionOptions = 1;
        public Action onHover;
        private Color? iconDrawColorOverride;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                if (actionOptions > 1)
                {
                    for (var i = 0; i < actionOptions; i++)
                    {
                        var index = i;
                        yield return new FloatMenuOption(floatMenuLabels?.Invoke(index) ?? string.Empty, () => action(index)); 
                    }
                }
            }
        }

        public override Color IconDrawColor
        {
            get
            {
                var color = iconDrawColorOverride;
                return color ?? base.IconDrawColor;
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            action(-1);
        }

        public override void GizmoUpdateOnMouseover() => onHover?.Invoke();

        public void SetColorOverride(Color color) => iconDrawColorOverride = color;
    }
}
