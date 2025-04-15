using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class Kever
{
	private double _x;
	private double _y;

    private double _beetleSpeed;
    private double _beetleSize;

	private double _totalDistanceCovered;
	
	public Kever(double xStart, double yStart, double speed, double size)
	{
		X = xStart; 
		Y = yStart;
		DX = 1;		//Possible values: -1 (move left), 1 (move right)
		DY = -1;	//Possible values: -1 (move up), 1 (move down)
		BeetleSpeed = speed;
		BeetleSize = size;
		TotalDistanceCovered = 0;
	}

	public double X { get { return _x; } set { if ((value >= 0) && (value <= 500)) { _x = value; } else {Console.WriteLine($"Error _x value: {value} out of bounds"); }; } }
	public double Y { get { return _y; } set { if ((value >= 0) && (value <= 300)) { _y = value; } else { Console.WriteLine($"Error _y value: {value} out of bounds"); }; }}
	
	public int DX { get; set; }
	public int DY { get; set; }
	
	public double BeetleSpeed { get { return _beetleSpeed; } set { if ((value >= 0.5) && (value <= 10)) { _beetleSpeed = value; } else { Console.WriteLine($"Error _beetleSpeed value: {value} out of bounds"); }; } } 
	public double BeetleSize { get { return _beetleSize; } set { if ((value >= 10) && (value <= 20)) { _beetleSize = value; } else { Console.WriteLine($"Error _beetleSize value: {value} out of bounds"); }; } }

	public double StepSize { get { return (BeetleSize * 0.1); } }		//computed property and  stepsize times 10 for test purposes, should be 0.01 
	public double TotalDistanceCovered { get { return _totalDistanceCovered; } set { _totalDistanceCovered = value; } }

	public void VisualizeBeetle(Canvas activeCanvas) 
	{
		Ellipse beetleBody = new Ellipse
		{
			Height = BeetleSize, 
			Width = BeetleSize,
			Fill = new SolidColorBrush(Colors.Red),
			
		};
        Canvas.SetLeft(beetleBody, X - BeetleSize / 2); //(X,Y) represents the center of beetlebody instead of topleft corner coordinates
        Canvas.SetTop(beetleBody, Y - BeetleSize / 2);
		activeCanvas.Children.Add(beetleBody);
	}

	public void AutoMove(Canvas canvas)
	{
		if (HitsVerticalBorder(canvas)) { DX *= -1;}		//If the beetle touches/exceeds the border, change direction via DX (1 *= -1 --> -1, -1 *= -1 --> 1)

		if (HitsHorizontalBorders(canvas)) { DY *= -1; }

		X += DX * StepSize;					//[ChatGPT code] new X = old X + (direction of movement * stepsize) 
		Y += DY * StepSize;					// example: X = 2 --> X = 2 + (1 * 0.2) --> X = 2.2
		TotalDistanceCovered += StepSize;					
	}

	public void MoveBeetle(string direction, Canvas canvas)	 
	{
		double r = BeetleSize / 2;
		bool moved = false;				//Changed inefficient code with use of bool to increase StepSize only if a step was made
		switch (direction.ToLower())
		{
			case "up":
				if (Y - r - StepSize >= 0) { Y -= StepSize; moved = true; }			//beetle is allowed to take a step as long as it does not exceed borders
				break;
			case "right":
				if (X + r + StepSize <= canvas.Width) { X += StepSize; moved = true; }
				break;
			case "down":
				if (Y + r + StepSize <= canvas.Height) { Y += StepSize; moved = true; }
				break;
			case "left":
				if (X - r - StepSize >= 0) { X -= StepSize; moved = true; }
				break;
		}
		if (moved) { TotalDistanceCovered += StepSize; }
	}

	private bool HitsVerticalBorder(Canvas canvas) 
	{
		int r = (int)(BeetleSize / 2);
		return (DX < 0 && X - r <= 0) || DX > 0 && (X + r) >= canvas.Width ;		//[ChatGPT] Detect collision left + right 
																	//kever beweegt nr L(DX) en raakt L || kever beweegt nr R(DX) en raakt R
	}

	private bool HitsHorizontalBorders(Canvas canvas)
	{
		int r = (int)(BeetleSize / 2);
		return (DY < 0 && Y - r <= 0) || DY > 0 && (Y + r) >= canvas.Height;	//[ChatGPT] Detect collision top + bottom 
																	//kever beweegt nr boven(DY) en raakt top || kever beweegt nr onder(DY) en raakt bodem
	}

    /* uitgeschreven border checks 
     * 
		bool raaktLinkerrand = false;
		bool raaktRechterrand = false;

		double r = BeetleSize / 2;
		
		if (DX < 0)
		{
			if (X - r <= 0)
			{
				raaktLinkerrand = true;
			}
		}

		if (DX > 0)
		{
			if (X + r >= canvas.Width)
			{
				raaktRechterrand = true;
			}
		}

		bool raaktRand = raaktLinkerrand || raaktRechterrand;
	*/
}
