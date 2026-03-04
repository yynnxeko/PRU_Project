using UnityEngine;

public class ObserveAreaStep : MissionStep
{
    public HoldAction hold = new HoldAction();

    bool isHolding;

    public override void UpdateStep()
    {
        // Bắt đầu giữ
        if (Input.GetKeyDown(KeyCode.E))
        {
            hold.StartHold();
            isHolding = true;
        }

        // Đang giữ
        if (isHolding && Input.GetKey(KeyCode.E))
        {
            bool completed = hold.UpdateHold(Time.deltaTime);
            if (completed)
            {
                IsCompleted = true;
                hold.CancelHold(); // khóa lại
            }
        }

        // Nhả phím
        if (Input.GetKeyUp(KeyCode.E))
        {
            hold.CancelHold();
            isHolding = false;
        }
    }

    public override void ResetStep()
    {
        base.ResetStep();
        hold.CancelHold();
        isHolding = false;
    }
}
