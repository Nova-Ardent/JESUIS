using JESUIS.Editor.Settings;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using JESUIS.Editor.Helpers;

namespace JESUIS.Editor.Elements.Common.Widgets
{
    public class Hierarchy : VisualElement
    {
        public const string HierarchyAssetPath = "Assets/JESUIS/Editor/Resources/Icons/Hierarchy/";

        Func<HierarchyItem, IEnumerable<NamedAction>> onRightClick;

        public HierarchyItem RootItem { get; private set; }

        public Hierarchy(object rootObject, Func<HierarchyItem, IEnumerable<NamedAction>> onRightClick)
        {
            this.onRightClick = onRightClick;

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
            style.backgroundColor = Colors.PANEL_COLOR;

            RootItem = new HierarchyItem(rootObject, onRightClick, AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(HierarchyAssetPath + "Root.png")), true);
            RootItem.SetHierarchyOwner(this);
            
            RebuildListVisuals();
        }

        public void RebuildListVisuals()
        {
            Clear();

            int index = 0;
            foreach (HierarchyItem item in RootItem.RecursivelyIterate())
            {
                Add(item);
                item.style.position = Position.Absolute;
                item.style.top = HierarchyItem.TOTAL_HEIGHT * index;
                index++;
            }
        }
    };

    public class HierarchyItem : VisualElement
    {
        public const int HEIGHT = 15;
        public const int ELEMENT_EDGE_PADDING = 1;

        public const int TOTAL_HEIGHT = HEIGHT + ELEMENT_EDGE_PADDING * 2;

        public const int INDENT_WIDTH = 25;
        public const int STARTING_INDENT_WIDTH = 40;
        public const int ELEMENT_SPLIT_PADDING = 3;

        Widgets.Hierarchy ownerHierarchy;
        Func<HierarchyItem, IEnumerable<NamedAction>> onRightClick;

        public HierarchyItem Parent { get; private set; }
        public List<HierarchyItem> ChildItems { get; private set; }

        bool isRoot = false;
        Label label;
        Image icon;
        CollapseIcon collapseIcon;

        int depth;

        public object TargetObject { get; private set; }

        public HierarchyItem(object targetObject, Func<HierarchyItem, IEnumerable<NamedAction>> onRightClick, bool isRoot = false) : this(targetObject, onRightClick, null, isRoot)
        {
        }

        public HierarchyItem(object targetObject, Func<HierarchyItem, IEnumerable<NamedAction>> onRightClick, Texture iconTexture, bool isRoot = false)
        {
            this.isRoot = isRoot;
            this.onRightClick = onRightClick;
            this.Parent = null;

            ChildItems = new List<HierarchyItem>();
            TargetObject = targetObject;

            SetIcon(iconTexture);
            SetLabel(targetObject.ToString());
            SetStyle();
            SetDepth(0);

            RegisterCallback<PointerEnterEvent>(OnMouseEnter);
            RegisterCallback<PointerLeaveEvent>(OnMouseLeave);
            RegisterCallback<PointerDownEvent>(OnMouseDown);

            SetDefaultStyle();
        }

        public void SetDepth(int depth)
        {
            this.depth = depth;

            label.style.left = (depth) * 25 + STARTING_INDENT_WIDTH;
            if (icon != null)
                icon.style.left = (depth) * 25 + STARTING_INDENT_WIDTH - HEIGHT - ELEMENT_SPLIT_PADDING;

            if (collapseIcon != null)
                collapseIcon.style.left = (depth) * 25 + STARTING_INDENT_WIDTH - HEIGHT - ELEMENT_SPLIT_PADDING - CollapseIcon.SIZE - ELEMENT_SPLIT_PADDING;
        }

        public bool IsCollapsed()
        {
            return collapseIcon != null && collapseIcon.IsCollapsed;
        }

        public void SetHierarchyOwner(Widgets.Hierarchy hierarchy)
        {
            ownerHierarchy = hierarchy;
        }

        public void AddChild(HierarchyItem child)
        {
            ChildItems.Add(child);
            child.SetDepth(depth + 1);
            child.SetHierarchyOwner(ownerHierarchy);
            child.Parent = this;

            if (collapseIcon == null)
            {
                AddCollapseIcon();
                collapseIcon.SetState(false);
            }
        }

        public void Remove()
        {
            Parent.ChildItems.Remove(this);
            if (Parent.ChildItems.Count == 0)
            {
                Parent.RemoveCollapseIcon();
            }
        }

        public IEnumerable<HierarchyItem> RecursivelyIterate()
        {
            yield return this;
            foreach (HierarchyItem child in ChildItems)
            {
                if (IsCollapsed())
                    continue;

                foreach (HierarchyItem subelements in child.RecursivelyIterate())
                {
                    yield return subelements;
                }
            }
        }

        void AddCollapseIcon()
        {
            collapseIcon = new CollapseIcon(OnCollapseChanged);
            collapseIcon.style.position = Position.Absolute;
            collapseIcon.style.top = (HEIGHT + ELEMENT_EDGE_PADDING - CollapseIcon.SIZE) / 2f;
            collapseIcon.style.left = (depth) * 25 + STARTING_INDENT_WIDTH - HEIGHT - ELEMENT_SPLIT_PADDING - CollapseIcon.SIZE - ELEMENT_SPLIT_PADDING;
            Add(collapseIcon);
        }

        void RemoveCollapseIcon()
        {
            if (collapseIcon != null)
            {
                Remove(collapseIcon);
                collapseIcon = null;
            }
        }

        void SetIcon(Texture iconTexture)
        {
            if (iconTexture != null)
            {
                icon = new Image();
                icon.image = iconTexture;
                icon.style.width = HEIGHT;
                icon.style.height = HEIGHT;
                icon.style.position = Position.Absolute;
                icon.style.top = ELEMENT_EDGE_PADDING;
                Add(icon);
            }
        }

        void SetLabel(string text)
        {
            label = new Label(text);
            label.text = text;
            label.style.position = Position.Absolute;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.height = HEIGHT;
            label.style.top = ELEMENT_EDGE_PADDING;
            label.style.left = ELEMENT_SPLIT_PADDING;
            label.style.right = 0;
            Add(label);
        }

        void SetStyle()
        {
            style.backgroundColor = Colors.HIERARCHY_ITEM_UNHIGHLIGHTED;
            style.height = TOTAL_HEIGHT;
            style.left = 0;
            style.right = 0;
        }

        void SetDefaultStyle()
        {
            if (isRoot)
            {
                style.backgroundColor = Colors.HIERARCHY_ROOT_ITEM_UNHIGHLIGHTED;
                style.borderBottomColor = Colors.HIERARCHY_ROOT_ITEM_TRIM;
                style.borderBottomWidth = 1;
            }
            else
                style.backgroundColor = Colors.HIERARCHY_ITEM_UNHIGHLIGHTED;
        }

        void OnMouseEnter(PointerEnterEvent pointerEnterEvent)
        {
            if (isRoot)
                style.backgroundColor = Colors.HIERARCHY_ROOT_ITEM_HIGHLIGHTED;
            else
                style.backgroundColor = Colors.HIERARCHY_ITEM_HIGHLIGHTED;
        }

        void OnMouseLeave(PointerLeaveEvent pointerLeaveEvent)
        {
            SetDefaultStyle();
        }

        void OnMouseDown(PointerDownEvent pointerDownEvent)
        {
            if (pointerDownEvent.button == 1)
            {
                if (onRightClick != null)
                {
                    ContextMenuBuilder.BuildMenu(onRightClick(this));
                }
                pointerDownEvent.StopImmediatePropagation();
            }
        }

        void OnCollapseChanged(bool isCollapsed)
        {
            ownerHierarchy.RebuildListVisuals();
        }
    }
}