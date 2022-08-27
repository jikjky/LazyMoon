using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyMoon.Class.Component.Slime
{
    public class SlimeModel : ComponentBase
    {
        [Parameter] public bool Debug { get; set; }
        [Parameter] public double X { get; set; }
        [Parameter] public double Y { get; set; }
        [Parameter] public double Speed { get; set; }
        [Parameter] public double frameDelay { get; set; }
        [Parameter] public string Image { get; set; }
        [Parameter] public string Control { get; set; }
        [Parameter] public bool Move { get; set; }

        internal (double x, double y) Position;
        internal (double x, double y) NewPosition;
        internal double Size = 25;
        private Random random = new Random();
        private long lastTime;
        private long lastJumpTime;

        private async Task<bool> StartAnimation()
        {

            lastTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            lastJumpTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            while (true)
            {
                await Task.Delay((int)frameDelay);

                if (Move == false)
                {
                    NewImage();
                }
                if (TuplesAreClose(Position, NewPosition))
                {
                    NewPosition = NewRandomPosition();
                    Console.WriteLine($"Aiming for {NewPosition.x:N2} , {NewPosition.y:N2}");
                }

                Update();

            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Position = (X, Y);
            NewPosition = Position;
            frameDelay = frameDelay == 0 ? 40 : frameDelay;
            Image = "Image/Slime/Idle.gif";
            StartAnimation().ConfigureAwait(false);
            return;
        }

        private void NewImage()
        {
            long thisTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            var time = thisTime - lastJumpTime;

            if (time > 3000)
            {
                lastJumpTime = thisTime;
                Random rd = new Random();
                var randomValue = rd.Next(0,5);
                if (randomValue == 0)
                    Image = "Image/Slime/Idle.gif";
                else if (randomValue == 1)
                    Image = "Image/Slime/jump.gif";
                else if (randomValue == 2)
                    Image = "Image/Slime/leftMove.gif";
                else if (randomValue == 3)
                    Image = "Image/Slime/rightMove.gif";
                else
                    Image = "Image/Slime/spin.gif";
            }
        }

        private (double x, double y) NewRandomPosition()
        {
            double rx = random.NextDouble() * 90;
            double ry = random.NextDouble() * 90;
            return (rx, ry);
        }

        private bool TuplesAreClose((double x, double y) A, (double x, double y) B)
        {
            bool close = Math.Abs(A.x - B.x) < 0.5 && Math.Abs(A.y - B.y) < 0.5;
            return close;
        }

        private void Update()
        {
            if (Move == false)
            {
                long thisTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                double dT = (double)(thisTime - lastTime) / frameDelay;
                lastTime = thisTime;

                double rx = NewPosition.x - Position.x;
                if (Math.Abs(rx) < 0.5)
                    rx = 0;
                //double xFix = Math.Abs(rx) < 5 ? (1 / (5 - Math.Abs(rx))) : 1;
                //double dx = Math.Max( Math.Min(Math.Sign(rx) * dT , xFix), Math.Sign(rx) * xFix);

                double ry = NewPosition.y - Position.y;
                if (Math.Abs(ry) < 0.5)
                    ry = 0;
                //double yFix = Math.Abs(ry) < 5 ? (1 / (5 - Math.Abs(ry))) : 1;
                //double dy = Math.Max(Math.Min(Math.Sign(ry) * dT, yFix), Math.Sign(ry) * yFix);

                double dx = Speed * dT * Math.Sign(rx) / (1000 / frameDelay);
                double dy = Speed * dT * Math.Sign(ry) / (1000 / frameDelay);
                if (Math.Abs(rx) < 5)
                    dx /= 3;
                if (Math.Abs(ry) < 5)
                    dy /= 3;

                Position = (Position.x + dx, Position.y + dy);
            }
            else
            {

                long thisTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                double dT = (double)(thisTime - lastTime) / frameDelay;
                lastTime = thisTime;

                var value = Control.Split('ㅤ').ToList();
                double dx = 0;
                double dy = 0;
                double speedValue = Speed;
                bool isShift = false;
                var a = (Size / (double)25);
                if (value.FirstOrDefault(x => x == "SHIFT") != null)
                {
                    speedValue = speedValue * 2;
                    isShift = true;
                }
                speedValue = speedValue * a;

                if (value.FirstOrDefault(x => x == "D") != null)
                {
                    dx += speedValue * dT / (1000 / frameDelay);
                }
                if (value.FirstOrDefault(x => x == "A") != null)
                {
                    dx -= speedValue * dT / (1000 / frameDelay);
                }
                if (value.FirstOrDefault(x => x == "S") != null)
                {
                    dy += speedValue * dT / (1000 / frameDelay);
                }
                if (value.FirstOrDefault(x => x == "W") != null)
                {
                    dy -= speedValue * dT / (1000 / frameDelay);
                }
                if (value.FirstOrDefault(x => x == "+") != null)
                {
                    Size += 1;
                }
                if (value.FirstOrDefault(x => x == "-") != null)
                {
                    Size -= 1;
                }
                if (Size > 100)
                {
                    Size = 100;
                }
                if (Size < 10)
                {
                    Size = 10;
                }

                long thisJumpTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                var time = thisJumpTime - lastJumpTime;
                if (value.FirstOrDefault(x => x == " ") != null)
                {
                    if(Image != "Image/Slime/jump.gif")
                    {
                        lastJumpTime = thisTime;
                        Image = "Image/Slime/jump.gif";
                    }
                }

                if (time > 3000)
                {
                    if (isShift == true)
                    {
                        Image = "Image/Slime/spin.gif";
                    }
                    else
                    {
                        if (dx < 0)
                        {
                            Image = "Image/Slime/leftMove.gif";
                        }
                        else if (dx > 0)
                        {
                            Image = "Image/Slime/rightMove.gif";
                        }
                        else
                        {
                            Image = "Image/Slime/Idle.gif";
                        }
                    }
                }

                Position = (Position.x + dx, Position.y + dy);

                if (Position.x < 5)
                {
                    Position.x = 5;
                }
                else if (Position.x > 90)
                {
                    Position.x = 90;
                }

                if (Position.y < 5)
                {
                    Position.y = 5;
                }
                else if (Position.y > 80)
                {
                    Position.y = 80;
                }
            }

            StateHasChanged();

        }

    }
}
