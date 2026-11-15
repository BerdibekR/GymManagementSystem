#include <math.h>

double calculate_workout_load(int duration_minutes, int intensity)
{
    if (duration_minutes <= 0)
        return -1.0;

    if (intensity < 1 || intensity > 3)
        return -1.0;

    double intensity_factor = 0.0;

    switch (intensity)
    {
    case 1:
        intensity_factor = 1.0;
        break;

    case 2:
        intensity_factor = 1.5;
        break;

    case 3:
        intensity_factor = 2.0;
        break;
    }

    return duration_minutes * intensity_factor;
}