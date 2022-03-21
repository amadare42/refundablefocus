using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RefundableFocus;

public class RefundView
{
    public Color RefundableTint = new Color(0.7148f, 0.7117f, 0.6604f, 0.572f);
    private readonly ColorBlock enforcedHoverColors;
    private readonly ColorBlock refundColors;

    public event Action<int> OnRefundRequested;
    private HashSet<CharacterOverworld> preventFocusHudUpdateFor = new();

    public RefundView()
    {
        this.refundColors = ColorBlock.defaultColorBlock;
        var hlColor = new Color(1.1f, 1.5f, 1.5f, 1.5f);
        refundColors.highlightedColor = hlColor;
        refundColors.colorMultiplier = 1.1f;

        this.enforcedHoverColors = ColorBlock.defaultColorBlock;
        enforcedHoverColors.highlightedColor = hlColor;
        enforcedHoverColors.normalColor = hlColor;
        enforcedHoverColors.colorMultiplier = 1.1f;

        On.uiPlayerMainHud.SetFocusMeter += OnSetFocusMeter;
    }

    private void OnSetFocusMeter(On.uiPlayerMainHud.orig_SetFocusMeter orig, uiPlayerMainHud self)
    {
        if (this.preventFocusHudUpdateFor.Contains(self.m_Cow))
        {
            this.preventFocusHudUpdateFor.Remove(self.m_Cow);
        }
        else
        {
            orig(self);
        }
    }

    public void SetAvailableRefund(CharacterOverworld cow, int refundableFocus)
    {
        preventFocusHudUpdateFor.Add(cow);
        SetAvailableRefundRoutine(cow, refundableFocus);
    }

    public void SetAvailableRefundRoutine(CharacterOverworld cow, int refundableFocus)
    {
        var focusBar = cow.m_UIPlayMainHud.transform.Find("DisplayRoot/focusBar");
        var currentFocus = cow.m_CharacterStats.m_FocusPoints;

        // starting with 1 because first entry is invisible focus slot presumably used as prefab
        for (var i = 1; i <= cow.m_CharacterStats.MaxFocus; i++)
        {
            var focusPointBG = focusBar.GetChild(i);
            if (!focusPointBG)
            {
                Plugin.Log.LogWarning($"Unexpected missing focus slots for {cow.m_CharacterStats.m_CharacterName}");
                break;
            }

            if (i <= currentFocus)
            {
                // should be filled normally
                RemoveRefund(focusPointBG, true);
            } 
            else if (i <= refundableFocus + currentFocus)
            {
                // should be refundable
                SetRefundable(focusPointBG, cow.IsOwner);
            }
            else
            {
                // should be empty
                RemoveRefund(focusPointBG, false);
            }
        }
    }

    public void SetRefundable(Transform focusPointBG, bool controllable)
    {
        var focusPointFill = focusPointBG.GetChild(0);
        var go = focusPointFill.gameObject;
        go.SetActive(true);
        
        // set tint
        var image = go.GetComponent<Image>();
        image.color = RefundableTint;

        // update button
        var button = go.GetComponent<Button>();
        if (!button)
        {
            button = go.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.onClick.AddListener(() => OnButtonClick(focusPointBG));
        }
        button.enabled = controllable;
        button.colors = this.refundColors;
        
        // update mouse listener
        var listener = go.GetComponent<PointerEventListener>();
        if (!listener)
        {
            listener = go.AddComponent<PointerEventListener>();
            listener.PointerEnter += OnPointerEnter;
            listener.PointerExit += OnPointerExit;
        }
        listener.enabled = controllable;
    }

    public void RemoveRefund(Transform focusPointBG, bool isFilled)
    {
        var focusPointFill = focusPointBG.GetChild(0);
        if (!focusPointFill) return;
        
        var go = focusPointFill.gameObject;
        go.SetActive(isFilled);
        var image = go.GetComponent<Image>();
        image.color = Color.white;
        
        var button = go.GetComponent<Button>();
        if (button) button.enabled = false;
        var listener = go.GetComponent<PointerEventListener>();
        if (listener) listener.enabled = false;
    }

    public void Clear(CharacterOverworld characterOverworld)
    {
        SetAvailableRefund(characterOverworld, 0);
    }

    private void OnPointerExit(PointerEventData data)
    {
        var focusPointFill = data.pointerEnter.transform;
        var focusPointBg = focusPointFill.parent;
        var focusBar = focusPointBg.parent;

        for (var i = 1; i <= focusPointBg.GetSiblingIndex(); i++)
        {
            var focusPoint = focusBar.GetChild(i);
            if (IsRefundableButton(focusPoint))
            {
                focusPoint.GetChild(0).GetComponent<Button>().colors = this.refundColors;
            }
        }
    }

    private void OnPointerEnter(PointerEventData data)
    {
        var focusPointFill = data.pointerEnter.transform;
        var focusPointBg = focusPointFill.parent;
        var focusBar = focusPointBg.parent;

        for (var i = 1; i <= focusPointBg.GetSiblingIndex(); i++)
        {
            var focusPoint = focusBar.GetChild(i);
            if (IsRefundableButton(focusPoint))
            {
                focusPoint.GetChild(0).GetComponent<Button>().colors = this.enforcedHoverColors;
            }
        }
    }
    
    private void OnButtonClick(Transform focusPointBg)
    {
        var focusBar = focusPointBg.parent;
        var count = 0;

        for (var i = 1; i <= focusPointBg.GetSiblingIndex(); i++)
        {
            var focusPoint = focusBar.GetChild(i);
            if (IsRefundableButton(focusPoint))
            {
                count++;
            }
        }
        
        OnRefundRequested?.Invoke(count);
    }

    public bool IsRefundableButton(Transform focusPointBG)
    {
        var focusPointFill = focusPointBG.GetChild(0);
        if (!focusPointFill) return false;
        var button = focusPointFill.GetComponent<Button>();
        if (!button) return false;
        return button.enabled;
    }
}