   M S B A M L   `   `   `  ����C  ?CharonUI, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:CharonUICharonUI  5   	   S OToggleSwitch, Version=1.1.2.0, Culture=neutral, PublicKeyToken=8637099990568f75A0clr-namespace:ToggleSwitch;assembly=ToggleSwitchToggleSwitch 5   	   M6clr-namespace:ToggleSwitch.Utils;assembly=ToggleSwitchToggleSwitch.Utils 5   	   Q8clr-namespace:ToggleSwitch.Borders;assembly=ToggleSwitchToggleSwitch.Borders 5	   	       CharonUI.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:CharonUI     5   	   IToggleSwitch0clr-namespace:ToggleSwitch;assembly=ToggleSwitch     5   	   HUtils6clr-namespace:ToggleSwitch.Utils;assembly=ToggleSwitch     5   	   LBorders8clr-namespace:ToggleSwitch.Borders;assembly=ToggleSwitch     5	   	   $	��355��5   	   $	��580��6     � WindowStyle$
  None=�6"    � AllowsTransparency . 
������  � 	Resources 5         GroovedThumbTemplate �� 
TargetType'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard g� Duration	 W� To	 �� TargetProperty
 �� 
TargetName*  $ToggleSwitch.Borders.OuterGlowBorder   OuterGlowSize   OuterGlowOpacity   OuterGlowColor   CornerRadius   ShadowCornerRadius)  #ToggleSwitch.Borders.ClippingBorder  Name   CornerRadius �� CornerRadius �� Padding �� Orientation J� Opacity ��Name J� IsHitTestVisible �� RadiusY �� RadiusX��%C  &	        �� 5   
   " n�67   .R�5      � 
������  5        5      CommonStates��. 5        
Normal��6&     5      	MouseOver��. 5      �� .��5   "   W� $ 0D�62   $
 0.05X�6?   $	 Opacity�6I   $
 BackgroundAnimation��6m   5      5        5      Pressed��. 5      �� .��5   "   W� $ 0D�62   $
 0.25X�6?   $	 Opacity�6I   $
 BackgroundAnimation��6m   5      5        5      Disabled��. 5      �� .��5   "   W� $ 0D�62   $	 .55X�6?   $	 Opacity�6H   $
 DisabledVisualElement��6l   5      5       5!        5"      FocusStates��. 5#        Focused��. 5$      �� .��5%   "   W� $ 0D�62   $
 0.05X�6?   $	 Opacity�6I   $
 FocusVisualElement��6m   5&      5'        5(      	Unfocused��6&   5)        5*      CheckStates��. 5+        	Unchecked��6&     5,      Checked��6&   5-      .+�5/        $ 4Q�6*   $
 0.15Q�6<   $��0q�6T   $ Black��6h   
������ $ 4��6�   $ 8��6�   ��.  .��50      � .+�51        $ 
Background��61   #��z��6E   #��z��6q   $ 4��6�   ��.  
�������.��52      �� ���	#FFEBEBEB$ 3��6=   #��z��6N   #��z��6x   $ 6q�6�   ._�53   "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�54   &   �� ���	#7F969696$��1��6A   $��0��6K   �� 55   &   ���	#7FF8F8F8$��1��6A   $��0��6K   $��0,0,1,0q�6_   �� 56   &   ���	#7F969696$��1��6A   $��0��6K   �� 57   &   ���	#7FF8F8F8$��1��6A   $��0��6K   $��0,0,1,0q�6_   �� 58   &   ���	#7F969696$��1��6A   $��0��6K   �� 59   &   ���	#7FF8F8F8$��1��6A   $��0��6K   5:   #   5;      5<        5=      $ BackgroundAnimation��61   
���   �$ 0Q�6a   #��z��6m   
���   �$ 4��6�   ��.  6�   �� 5>      $ DisabledVisualElement��6$   
������� .  $ 0Q�6i   $ 4��6u   $ 4��6�   �� 5?      $ FocusVisualElement��6$    .  $��1q�6Y   $ 0Q�6d   $ 4��6p   $ 4��6|   $��1��6�   5@      5A      5B      5C      5G      .��5K      �� ���	#FFA50505$��0,0,0,0q�6$   $ 10,10,10,10��6>   $��0,0,0,0q�6Y   ._�5M   
   �� $��0,0,0,0q�6   .+�5O      �� -   5        $��522,10,0,10q�5O      $��25��6*   $��25��66   $��X��6A    J� RenderTransformOrigin$ 1.6,0.4&�6b   �� 5Q      $��	35,5,35,0q�6   $ 5q�6(   ���	#FF34302F$��3,5,3,5q�6L   $ 10��6f   $	��Top=�6x   $	��270��6�   ��5R      �� 5X       �� ImageSource$ 
bk-gnd.bmp��6!   6<   ._�5[      � $��8,5,2,1q�6    � ColumnDefinitions 5]      �� 5^      $
��162* �6+   �� 5_      $
��218* �6+   �� 5`      $
��105* �6+    � RowDefinitions 5b      �� 5c      $	��30* �6(   �� 5d      $	��35* �6(   �� 5e      $	��30* �6(   �� 5f      $	��30* �6(   �� 5g      $	��35* �6(   �� 5h      $	��40* �6(   �� 5i      $	��44* �6(   .+�5m      �� -   5         ��Name$ lblDate��5m   1   $��0,9,1,0q�6    &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=�6B   $��Right=�6k   $	��Top=�6�   $
��Bold"�6�   $	��240��6�   $��1��6�   $��0��6�   
�������$��17��6�   $��2��6�   �� 5n      -   5          ��Name$  lblSunriseVal��5n      $��07:05 AM��63   $��2��6F   $��Right=�6V   $��1��6r   $��70��6   $��24��6�   ���	#FFFAFAFA$	��Top=�6�   $	��5,2q�6�   $��0,7,33,0q�6�   $
��Bold"�6�   �� 5o      -   5        $  
lblSunrise��5o      $��Today's Sunrise:��60   $��1��6K   $��Right=�6[   $��0,7,0,4q�6w   $��1��6�   $��Center=�6�   $	��120��6�   $��24��6�   ���	#FFFAFAFA$��5,2,5,5q�6�   $
��Bold"�6�   �� 5p      -   5        $  	lblSunset��5p      $��Today's Sunset:��6/   $��1��6I   $��Right=�6Y   $��0,1,1,0q�6u   $��2��6�   $	��Top=�6�   $	��120��6�   
�������$��26��6�   $��5,2,5,5q�6�   $
��Bold"�6�   �� 5q      -   5        $  lblSunsetVal��5q      $��06:15 PM��62   $��2��6E   $��Right=�6U   $��0,1,34,0q�6q   $��2��6�   $	��Top=�6�   $��70��6�   $��24��6�   
�������$	��5,2q�6�   $
��Bold"�6�   )  #ToggleSwitch.HorizontalToggleSwitch  5r      -   5        !  Name$! BtnDCBus��5r   :   #  ToggleSwitch.ToggleSwitchBase"   	IsChecked" . $
��Left=�6]   $��2��6x   $��5��6�   $��9,12,0,4q�6�   $��24��6�     5s      -   5        $! BtnACBus��5s   :   " . $
��Left=�6]   $��2��6x   $��6��6�   $��9,4,0,16q�6�   $��24��6�   �� 5t      -	   5        $  lblDCBus��5t      $��DC Bus:��6.   $��1��6@   $��0,12,1,0q�6P   $��5��6b   $	��Top=�6o   $��26��6�   
�������$��0,5,0,0q�6�   $
��Bold"�6�   $��50��6�   $��Right=�6�   �� 5u      -
   5        $  lblACBus��5u      $��AC Bus:��6.   $��1��6@   $��Right=�6P   $��0,1,1,0q�6l   $��6��6}   $	��Top=�6�   $��50��6�   $��26��6�   $
��Bold"�6�   
�������$��0,5,0,0q�6�   5z      5|      5}      5�      5�      