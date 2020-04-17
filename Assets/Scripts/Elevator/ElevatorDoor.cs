using UnityEngine;

public class ElevatorDoor : DoorManager
{
    public int floor;               // The floor this door is on.
    public Elevator elevator;       // Reference to the elevator for this door.


	protected override bool ShouldOpenForCollider (Collider col)
	{
        // The door should open for a collider if the elevator is not moving and it's the player.
		return !elevator.ElevatorMoving && col.CompareTag ("Player");
	}


    protected override bool ShouldBeOpen 
    {
        // In addition to the normal conditions for being open, the elevator must not be moving and must be at the floor of this door.
		get { return base.ShouldBeOpen && !elevator.ElevatorMoving && elevator.CurrentFloor == floor; }
	}


    public override void Update ()
    {
        // If the door should normally be open, the elevator is not moving but it's not on the correct floor...
        if (base.ShouldBeOpen && !elevator.ElevatorMoving && elevator.CurrentFloor != floor)
        {
            // ... call the elevator.
            elevator.CallElevator (floor);
        }
        
        base.Update ();
    }
}
