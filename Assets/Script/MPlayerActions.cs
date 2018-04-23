using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MPlayerActions : PlayerActionSet  {
    
	//public PlayerAction Touch;
	//public PlayerAction SwitchMode;
//	public PlayerAction Jump;
//	public PlayerAction Run;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Move;

    public PlayerAction Plant;
    public PlayerAction Dash;
    public PlayerAction SwitchPlant;
	//public PlayerAction ScreenLeft;
	//public PlayerAction ScreenRight;
	//public PlayerAction ScreenUp;
	//public PlayerAction ScreenDown;
	//public PlayerTwoAxisAction Screen;
	//public PlayerAction PlayMusicOne;
	//public PlayerAction PlayMusicTwo;
	//public PlayerAction PlayMusicThree;
	//public PlayerAction PlayMusicFour;
	//public PlayerAction PlayMusicShake;
	//public PlayerAction PlayMusicHold;
	//public PlayerAction Interact;
	//public PlayerAction SwitchMusicMode;
	//public PlayerAction ToggleMusicPanel;
	//public PlayerAction MusicNext;
	//public PlayerAction MusicLast;

	public PlayerAction Confirm;


	public MPlayerActions(){
		//Touch = CreatePlayerAction ("Touch");
		//SwitchMode = CreatePlayerAction ("PlayMusic");

//		Jump = CreatePlayerAction( "Jump" );
//		Run = CreatePlayerAction ("Run");
		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
        Plant = CreatePlayerAction("Plant");
        Dash = CreatePlayerAction("Dash");
        SwitchPlant = CreatePlayerAction("SwitchPlant");

	}

	public static MPlayerActions CreateWithDefaultBindings()
	{
		var playerActions = new MPlayerActions ();

		//playerActions.Run.AddDefaultBinding (Key.Shift);
		//playerActions.Run.AddDefaultBinding (InputControlType.RightBumper);
		//playerActions.Run.AddDefaultBinding (InputControlType.LeftBumper);

		//playerActions.SwitchMode.AddDefaultBinding ( InputControlType.TouchPadButton );
		//playerActions.SwitchMode.AddDefaultBinding ( Key.Space );

		playerActions.Up.AddDefaultBinding( Key.W  );
		playerActions.Down.AddDefaultBinding( Key.S  );
		playerActions.Left.AddDefaultBinding( Key.A );
		playerActions.Right.AddDefaultBinding( Key.D  );

		playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
		playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );
		playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
		playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

		playerActions.Left.AddDefaultBinding( InputControlType.DPadLeft );
		playerActions.Right.AddDefaultBinding( InputControlType.DPadRight );
		playerActions.Up.AddDefaultBinding( InputControlType.DPadUp );
		playerActions.Down.AddDefaultBinding( InputControlType.DPadDown );

        playerActions.Dash.AddDefaultBinding(Key.Space);
        playerActions.Dash.AddDefaultBinding(InputControlType.Action1);

        playerActions.Plant.AddDefaultBinding(Key.E);
        playerActions.Plant.AddDefaultBinding(InputControlType.Action2);

        playerActions.SwitchPlant.AddDefaultBinding(Key.Tab);
        playerActions.SwitchPlant.AddDefaultBinding(InputControlType.RightBumper);
        playerActions.SwitchPlant.AddDefaultBinding(InputControlType.LeftBumper);


        return playerActions;
	}
}
