/*
 * Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

#import "FlippedView.h"

/** @class FlippedView
 * This view is simply a NSView with flipped coordinates,
 * with (0,0) indicating the top-left corner instead of the
 * bottom-left corner.
 */
@implementation FlippedView

- (BOOL)isFlipped {
    return YES;
}

- (BOOL)autoresizesSubviews {
    return YES;
}

@end


