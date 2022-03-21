using System;
using GridEditor;
using static RefundableFocus.Common.Utils;

namespace RefundableFocus;

public class FocusTracker
{
    private readonly RefundView view;

    public int RefundableFocus = 0;
    public CharacterOverworld Cow = null;

    public FocusTracker(RefundView view)
    {
        this.view = view;
        On.Movement.ConvertFocusToAction += OnConvertFocus;
        On.CharacterOverworld.EndTurn += OnCowEndTurn;
        On.CharacterOverworld.OnMoveOneHexFinish += OnMoveOneHexFinish;
        this.view.OnRefundRequested += Refund;
    }

    private void OnMoveOneHexFinish(On.CharacterOverworld.orig_OnMoveOneHexFinish orig, CharacterOverworld self, GameLogic.EncounterType _drawencounter, bool _decactionpoint)
    {
        SafeInvoke(() => orig(self, _drawencounter, _decactionpoint), () => {
            if (_decactionpoint && self == this.Cow)
            {
                this.RefundableFocus = Math.Min(this.RefundableFocus, self.m_CharacterStats.m_ActionPoints);
                this.view.SetAvailableRefund(self, this.RefundableFocus);
            }
        });
    }

    private void OnConvertFocus(On.Movement.orig_ConvertFocusToAction orig, Movement self)
    {
        var wouldApply = !(self.m_CharacterOverworld.m_CharacterStats.m_FocusPoints <= 0 ||
                           self.m_CharacterOverworld.m_CharacterStats.m_ActionPoints >= 9);
        SafeInvoke(() => orig(self), () =>
        {
            if (wouldApply) IncrementFocusSpent(self.m_CharacterOverworld);
        });
    }

    private void OnCowEndTurn(On.CharacterOverworld.orig_EndTurn orig, CharacterOverworld self)
    {
        SafeInvoke(() => orig(self), Reset);
    }

    public void IncrementFocusSpent(CharacterOverworld cow)
    {
        if (cow != Cow)
        {
            this.Cow = cow;
            this.RefundableFocus = 0;
        }

        this.RefundableFocus++;
        this.view.SetAvailableRefund(cow, this.RefundableFocus);
    }

    public void Reset()
    {
        if (this.Cow != null && this.Cow)
        {
            this.view.SetAvailableRefund(this.Cow, 0);
        }

        this.Cow = null;
        this.RefundableFocus = 0;
    }

    public void Refund(int count)
    {
        if (this.Cow == null)
        {
            Plugin.Log.LogWarning($"Cannot refund {count} Focus - COW is missing");
            return;
        }

        count = Math.Min(count, this.RefundableFocus);
        this.Cow.m_CharacterStats.UpdateFocusPoints(count);
        this.Cow.UpdatePlayerAction(-count);
        if ((bool) (UnityEngine.Object) this.Cow.GetCurrentDummy())
            this.Cow.GetCurrentDummy().PlayCharacterAbilityEvent(FTK_characterSkill.ID.Refocus);
        else
            this.Cow.PlayCharacterAbilityEvent(FTK_characterSkill.ID.Refocus);
        Movement.Instance.TrackResetList();
        
        this.RefundableFocus -= count;
        this.view.SetAvailableRefund(this.Cow, this.RefundableFocus);
    }
}