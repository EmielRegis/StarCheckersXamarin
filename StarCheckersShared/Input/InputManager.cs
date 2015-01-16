using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace StarCheckersWindows
{
    public class InputManager
    {
		public int MouseOrTouchX { get; set;}
		public int MouseOrTouchY { get; set; }
		public bool IsTouch { get; private set;}
        private KeyboardState currentKeyState, prevKeyState;
        private MouseState currentMouseState, prevMouseState;
		private TouchCollection currentTouchState, prevTouchState;
        private static InputManager instance;

        public static InputManager Instance
        {
            get { return instance ?? (instance = new InputManager()); }
        }

		public InputManager()
		{
			TouchPanel.EnableMouseGestures = true;
			TouchPanel.EnableMouseGestures = true;
		}

        public void Update()
        {
            prevKeyState = currentKeyState;
            prevMouseState = currentMouseState;
			prevTouchState = currentTouchState;
            if (!ScreenManager.Instance.IsTransitioning)
            {
                currentKeyState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
				currentTouchState = TouchPanel.GetState();
            }
        }

		public void SetTouch(int x, int y)
		{
			MouseOrTouchX = x;
			MouseOrTouchY = y;
			IsTouch = true;
		}

		public void HandleTouch()
		{
			IsTouch = false;
		}

        public bool MouseLeftButtonPressed()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed &&
                    prevMouseState.LeftButton == ButtonState.Released);
        }

        public bool MouseLeftButtonReleased()
        {
            return (currentMouseState.LeftButton == ButtonState.Released &&
                    prevMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool MouseLeftButtonDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool MouseRightButtonPressed()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed &&
                    prevMouseState.RightButton == ButtonState.Released);
        }

        public bool MouseRightButtonReleased()
        {
            return (currentMouseState.RightButton == ButtonState.Released &&
                    prevMouseState.RightButton == ButtonState.Pressed);
        }

        public bool MouseRightButtonDown()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool MouseMiddleLeftButtonPressed()
        {
            return (currentMouseState.MiddleButton == ButtonState.Pressed &&
                    prevMouseState.MiddleButton == ButtonState.Released);
        }

        public bool MouseMiddleButtonReleased()
        {
            return (currentMouseState.MiddleButton == ButtonState.Released &&
                    prevMouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool MouseMiddleButtonDown()
        {
            return currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool KeyPressed(params Keys[] keys)
        {
            return keys.Any(k => currentKeyState.IsKeyDown(k) && prevKeyState.IsKeyUp(k));
        }

        public bool KeyReleased(params Keys[] keys)
        {
            return keys.Any(k => currentKeyState.IsKeyUp(k) && prevKeyState.IsKeyDown(k));
        }

        public bool KeyDown(params Keys[] keys)
        {
            return keys.Any(currentKeyState.IsKeyDown);
        }
    }
}
