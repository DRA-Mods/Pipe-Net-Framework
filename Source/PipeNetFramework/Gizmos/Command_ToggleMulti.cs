using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace PipeNetFramework.Gizmos
{
    public class Command_ToggleMulti : Command
    {
        public Action<int> toggleAction;
        public Func<int, MultiCheckboxState> isActive;
        public Func<int, string> floatMenuLabels;
        public int totalActions = 1;

        public SoundDef turnOnSound = SoundDefOf.Checkbox_TurnedOn;
        public SoundDef turnOffSound = SoundDefOf.Checkbox_TurnedOff;
        public SoundDef turnPartialSound = SoundDefOf.Checkbox_TurnedOff;

        public override SoundDef CurActivateSound
            => isActive(-1) switch
            {
                MultiCheckboxState.On => turnOnSound,
                MultiCheckboxState.Off => turnOffSound,
                MultiCheckboxState.Partial => turnPartialSound,
                _ => turnPartialSound
            };

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                if (totalActions > 1)
                {
                    for (var i = 0; i < totalActions; i++)
                    {
                        var index = i;
                        yield return new FloatMenuOption(floatMenuLabels?.Invoke(i) ?? string.Empty, () => toggleAction(index), extraPartOnGUI: (rect) => DrawIconFloatMenu(rect, index));
                    }
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            toggleAction(-1);
        }

        public override GizmoResult GizmoOnGUI(Vector2 loc, float maxWidth, GizmoRenderParms parms)
        {
            var result = base.GizmoOnGUI(loc, maxWidth, parms);
            var rect = new Rect(loc.x, loc.y, GetWidth(maxWidth), 75f);
            var position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
            var image = isActive(-1) switch
            {
                MultiCheckboxState.On => Widgets.CheckboxOnTex,
                MultiCheckboxState.Off => Widgets.CheckboxOffTex,
                MultiCheckboxState.Partial => Widgets.CheckboxPartialTex,
                _ => Widgets.CheckboxPartialTex
            };

            GUI.DrawTexture(position, image);

            return result;
        }

        public virtual bool DrawIconFloatMenu(Rect rect, int index)
        {
            var image = isActive(index) switch
            {
                MultiCheckboxState.On => Widgets.CheckboxOnTex,
                MultiCheckboxState.Off => Widgets.CheckboxOffTex,
                MultiCheckboxState.Partial => Widgets.CheckboxPartialTex,
                _ => Widgets.CheckboxPartialTex
            };
            var size = rect.height - 5f;
            var halfSize = rect.height / 2f;
            var target = new Rect(rect.xMax - halfSize, rect.yMax - halfSize, size, size);
            GUI.DrawTexture(target, image);

            return false;
        }

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            return other is Command_ToggleMulti toggle && toggle.isActive(-1) == isActive(-1);
        }
    }
}
