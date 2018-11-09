using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity
{
	
	public static Dictionary<Direction, Vector3> vectors = new Dictionary<Direction, Vector3> () {
		{ Direction.Down, Vector3.down },
		{ Direction.Up, Vector3.up },
		{ Direction.Left, Vector3.left },
		{ Direction.Right, Vector3.right },
		{ Direction.Front, Vector3.forward },
		{ Direction.Back, Vector3.back },
		{ Direction.Neutral, Vector3.zero }
	};

	public static Transition reverseTransition (Transition t1)
	{
		switch (t1) {
		case Transition.X_CCW:
			return Transition.X_CW;
		case Transition.X_CW:
			return Transition.X_CCW;
		case Transition.Y_CCW:
			return Transition.Y_CW;
		case Transition.Y_CW:
			return Transition.Y_CCW;
		case Transition.Z_CCW:
			return Transition.Z_CW;
		case Transition.Z_CW:
			return Transition.Z_CCW;
		}
		return t1;
	}

	public static Direction reverseDirection (Direction d1)
	{
		switch (d1) {
		case Direction.Down:
			return Direction.Up;
		case Direction.Up:
			return Direction.Down;
		case Direction.Left:
			return Direction.Right;
		case Direction.Right:
			return Direction.Left;
		case Direction.Front:
			return Direction.Back;
		case Direction.Back:
			return Direction.Front;
		}
		return d1;
	}

	public static Transition getTransition (Direction d1, Direction d2)
	{
		if (d1 == d2)
			return Transition.None;
		switch (d1) {
		case Direction.Down:
			switch (d2) {
			case Direction.Left:
				return Transition.Z_CW;
			case Direction.Right:
				return Transition.Z_CCW;
			case Direction.Front:
				return Transition.X_CW;
			case Direction.Back:
				return Transition.X_CCW;
			case Direction.Up:
				return Transition.Y_Flip;
			}
			break;
		case Direction.Up:
			return getTransition (Direction.Down, reverseDirection (d2));
		case Direction.Left:
			switch (d2) {
			case Direction.Down:
				return Transition.Z_CCW;
			case Direction.Up:
				return Transition.Z_CW;
			case Direction.Front:
				return Transition.Y_CCW;
			case Direction.Back:
				return Transition.Y_CW;
			case Direction.Right:
				return Transition.X_Flip;
			}
			break;
		case Direction.Right:
			return getTransition (Direction.Left, reverseDirection (d2));
		case Direction.Front:
			switch (d2) {
			case Direction.Down:
				return Transition.X_CCW;
			case Direction.Up:
				return Transition.X_CW;
			case Direction.Left:
				return Transition.Y_CW;
			case Direction.Right:
				return Transition.Y_CCW;
			case Direction.Back:
				return Transition.Z_Flip;
			}
			break;
		case Direction.Back:
			return getTransition (Direction.Front, reverseDirection (d2));
		}
		return Transition.Unknown;
	}

	public static Direction applyTransition (Transition t1, Direction d1)
	{
		switch (t1) {
		case Transition.None:
		case Transition.Unknown:
			return d1;
		case Transition.X_Flip:
			return (d1 == Direction.Left || d1 == Direction.Right) ? reverseDirection(d1) : d1;
		case Transition.Y_Flip:
			return (d1 == Direction.Down || d1 == Direction.Up) ? reverseDirection(d1) : d1;
		case Transition.Z_Flip:
			return (d1 == Direction.Front || d1 == Direction.Back) ? reverseDirection(d1) : d1;
		case Transition.X_CW:
			switch (d1) {
			case Direction.Down:
				return Direction.Front;
			case Direction.Up:
				return Direction.Back;
			case Direction.Front:
				return Direction.Up;
			case Direction.Back:
				return Direction.Down;
			}
			break;
		case Transition.X_CCW:
			switch (d1) {
			case Direction.Down:
			case Direction.Up:
			case Direction.Front:
			case Direction.Back:
				return applyTransition (Transition.X_CW, reverseDirection (d1));
			}
			break;
		case Transition.Y_CW:
			switch (d1) {
			case Direction.Left:
				return Direction.Back;
			case Direction.Right:
				return Direction.Front;
			case Direction.Front:
				return Direction.Left;
			case Direction.Back:
				return Direction.Right;
			}
			break;
		case Transition.Y_CCW:
			switch (d1) {
			case Direction.Left:
			case Direction.Right:
			case Direction.Front:
			case Direction.Back:
				return applyTransition (Transition.Y_CW, reverseDirection (d1));
			}
			break;
		case Transition.Z_CW:
			switch (d1) {
			case Direction.Down:
				return Direction.Left;
			case Direction.Up:
				return Direction.Right;
			case Direction.Left:
				return Direction.Up;
			case Direction.Right:
				return Direction.Down;
			}
			break;
		case Transition.Z_CCW:
			switch (d1) {
			case Direction.Down:
			case Direction.Up:
			case Direction.Left:
			case Direction.Right:
				return applyTransition (Transition.Z_CW, reverseDirection (d1));
			}
			break;
		}
		return d1;
	}

	public enum Direction
	{
		Down,
		Up,
		Left,
		Right,
		Front,
		Back,
		Neutral
	}

	public enum Transition
	{
		None,
		// looking towards positive X, clockwise
		X_CW,
		X_CCW,
		X_Flip,
		Y_CW,
		Y_CCW,
		Y_Flip,
		Z_CW,
		Z_CCW,
		Z_Flip,
		Unknown
	}

}
