﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Framework.Input.States;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Objects.Drawables;
using OpenTK;

namespace osu.Game.Rulesets.Edit
{
    /// <summary>
    /// A mask placed above a <see cref="DrawableHitObject"/> adding editing functionality.
    /// </summary>
    public class SelectionMask : CompositeDrawable, IStateful<SelectionState>
    {
        /// <summary>
        /// Invoked when this <see cref="SelectionMask"/> has been selected.
        /// </summary>
        public event Action<SelectionMask> Selected;

        /// <summary>
        /// Invoked when this <see cref="SelectionMask"/> has been deselected.
        /// </summary>
        public event Action<SelectionMask> Deselected;

        /// <summary>
        /// Invoked when this <see cref="SelectionMask"/> has requested selection.
        /// Will fire even if already selected. Does not actually perform selection.
        /// </summary>
        public event Action<SelectionMask, InputState> SelectionRequested;

        /// <summary>
        /// Invoked when this <see cref="SelectionMask"/> has requested drag.
        /// </summary>
        public event Action<SelectionMask, Vector2, InputState> DragRequested;

        /// <summary>
        /// The <see cref="DrawableHitObject"/> which this <see cref="SelectionMask"/> applies to.
        /// </summary>
        public readonly DrawableHitObject HitObject;

        protected override bool ShouldBeAlive => HitObject.IsAlive && HitObject.IsPresent || State == SelectionState.Selected;
        public override bool HandlePositionalInput => ShouldBeAlive;
        public override bool RemoveWhenNotAlive => false;

        public SelectionMask(DrawableHitObject hitObject)
        {
            HitObject = hitObject;

            RelativeSizeAxes = Axes.Both;

            AlwaysPresent = true;
            Alpha = 0;
        }

        private SelectionState state;

        public event Action<SelectionState> StateChanged;

        public SelectionState State
        {
            get => state;
            set
            {
                if (state == value) return;

                state = value;
                switch (state)
                {
                    case SelectionState.Selected:
                        Show();
                        Selected?.Invoke(this);
                        break;
                    case SelectionState.NotSelected:
                        Hide();
                        Deselected?.Invoke(this);
                        break;
                }
            }
        }

        /// <summary>
        /// Selects this <see cref="SelectionMask"/>, causing it to become visible.
        /// </summary>
        public void Select() => State = SelectionState.Selected;

        /// <summary>
        /// Deselects this <see cref="SelectionMask"/>, causing it to become invisible.
        /// </summary>
        public void Deselect() => State = SelectionState.NotSelected;

        public bool IsSelected => State == SelectionState.Selected;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => HitObject.ReceivePositionalInputAt(screenSpacePos);

        private bool selectionRequested;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            selectionRequested = false;

            if (State == SelectionState.NotSelected)
            {
                SelectionRequested?.Invoke(this, e.CurrentState);
                selectionRequested = true;
            }

            return IsSelected;
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (State == SelectionState.Selected && !selectionRequested)
            {
                selectionRequested = true;
                SelectionRequested?.Invoke(this, e.CurrentState);
                return true;
            }

            return base.OnClick(e);
        }

        protected override bool OnDragStart(DragStartEvent e) => true;

        protected override bool OnDrag(DragEvent e)
        {
            DragRequested?.Invoke(this, e.Delta, e.CurrentState);
            return true;
        }

        /// <summary>
        /// The screen-space point that causes this <see cref="SelectionMask"/> to be selected.
        /// </summary>
        public virtual Vector2 SelectionPoint => HitObject.ScreenSpaceDrawQuad.Centre;

        /// <summary>
        /// The screen-space quad that outlines this <see cref="SelectionMask"/> for selections.
        /// </summary>
        public virtual Quad SelectionQuad => HitObject.ScreenSpaceDrawQuad;
    }
}