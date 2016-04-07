﻿using System;
using System.ComponentModel.Design.Serialization;

namespace GraphicsSandbox
{
    public class Collision : TimeDependentAction {
        private readonly double _distance;
        private readonly Element _e1;
        private readonly Element _e2;

        public Collision(double distance, Element e1, Element e2)
        {
            _distance = distance;
            _e1 = e1;
            _e2 = e2;
        }

        public override void Act()
        {
            if (_e1 == _e2)
            {
                throw new Exception("Cant collide with self");
            }

            var distance = _e1.Location - _e2.Location;
            if ( distance.Magnitude > _distance ) {
                return;
            }

            Dimensions moveBy = distance.Unit * _distance; // double up.

            _e1.Location -= moveBy;
            _e2.Location += moveBy;

            _e1.Velocity.Dimensions = _e1.Velocity.Dimensions * distance.Unit * -1;
            _e2.Velocity.Dimensions = _e2.Velocity.Dimensions * distance.Unit;

        }
    }
}