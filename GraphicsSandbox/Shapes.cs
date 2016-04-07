﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace GraphicsSandbox {
    public class Dimensions {
        public Dimensions(double x, double y)
        {
            X = x;
            Y = y;
        }

        public readonly double X;
        public readonly double Y;
        private double? _magnitude;
        private Dimensions _unit;

        public static Dimensions operator * (Dimensions subject, double multiplyBy){
            return new Dimensions(subject.X * multiplyBy, subject.Y * multiplyBy);
        }

        public static Dimensions operator /(Dimensions subject, double divideBy) {
            return new Dimensions(subject.X / divideBy, subject.Y / divideBy);
        }

        public static Dimensions operator *(Dimensions subject, Dimensions multiplyBy) {
            return new Dimensions(subject.X * multiplyBy.X, subject.Y * multiplyBy.Y);
        }

        public static Dimensions operator +(Dimensions subject, Dimensions multiplyBy) {
            return new Dimensions(subject.X + multiplyBy.X, subject.Y + multiplyBy.Y);
        }

        public static Dimensions operator -(Dimensions subject, Dimensions multiplyBy) {
            return new Dimensions(subject.X - multiplyBy.X, subject.Y - multiplyBy.Y);
        }

        public double Magnitude
        {
            get
            {
                if (_magnitude == null)
                {
                    _magnitude = Math.Pow(
                    Math.Pow(X, 2) + Math.Pow(X, 2)
                    , 0.5
                    );
                }

                return _magnitude.Value; 
            }
        }

        public Dimensions Unit {
            get {
                
                if (_unit == null) 
                {
                    _unit = Magnitude == 0d ? new Dimensions(1, 1) : this / Magnitude;
                }
                return _unit;
            }
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
