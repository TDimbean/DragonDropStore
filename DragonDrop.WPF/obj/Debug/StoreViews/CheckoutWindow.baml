   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:DragonDrop.WPF.StoreViewsDragonDrop.WPF.StoreViews  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   e7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	   	   /clr-namespace:DragonDrop.WPFDragonDrop.WPF  5
   	   .    (DragonDrop.WPF.StoreViews.CheckoutWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:DragonDrop.WPF.StoreViews     5   	   5my&clr-namespace:System;assembly=mscorlib     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5	   	   ,behclr-namespace:DragonDrop.WPF     5
   	   -   5          � Title$  CheckoutWindow��5   	   $	��480��6     � SizeToContent$ Height=�6,    � 
ResizeMode$ NoResize=�6C    � WindowStyle$ SingleBorderWindow=�6Y    � WindowStartupLocation$ CenterScreen=�5   	   .��5      ��  �� Orientation$ Vertical=�6   $��Center=�6(    � 	Resources 5   
      FieldImgHeightJ   DDragonDrop.WPF.StoreViews.Tabs.Converters.IdToPaymentMethodConverter  PayMethConvK   EDragonDrop.WPF.StoreViews.Tabs.Converters.IdToShippingMethodConverter  ShipMethConv �� 
TargetType �� Property	 �� Value&   System.Windows.Controls.DataGrid5  /System.Windows.Controls.DataGridLengthConverter��%   &	        &	      &	 -     '�� @     '�� �     '�� L    '  �    X�5      30Q�63     5      6.     5      6/   �� 5      " ��6   .�5      ��  � ��	 6/   Z� �� 5       � ��	 60   Z� 5      �� 5      " ��6   .�5      ��  �@��DisplayMemberPath$
	 Name��66   �� 5       �@��
IsEditable	 . 6/   �� 5       �@��
IsReadOnly	 . 6/   �� 5       � ��$	 18(�6-   �� 5       � ��$		 200��6*   �� 5       � ��$	 Center=�68   �� 5       � ��$	 Center=�66   �� 5        � ��$	 Center=�6?   �� 5!       � ��$	 0 6 0 0q�6+   5"      �� 5#      " ��6   .�5$      ��  � ��$	 Center=�66   �� 5%       �@��StrokeDashArray$	 2R�64   �� 5&       � ��$	 3��64   �� 5'       � ��
	 �   �6+   �� 5(       � ��
	 �   �6)   �� 5)      
 �@��X1$	 0��6'   �� 5*      
 �@��X2$		 160��6'   �� 5+      
 �@��Y1$	 0��6'   �� 5,      
 �@��Y2$	 0��6'   5-      �� 5.      "  6   .�5/      ��  �@ GridLinesVisibility$	 Vertical=�68   �� 50       �@ CanUserAddRows	 .  63   �� 51       �@ CanUserDeleteRows	 .  66   �� 52       �@ AutoGenerateColumns	 .  68   �� 53       � ��$	 Center=�6?   �� 54       �@ ColumnWidth$	 * 60   �� 55       �@ CanUserReorderColumns	 .  6:   �� 56       �@ CanUserResizeColumns	 .  69   �� 57       �@ CanUserResizeRows	 .  66   �� 58       �@ CanUserSortColumns	 .  67   �� 59       �@ 
IsReadOnly	 . 6/   �� 5:       � ��$	 Stretch=�66   �� 5;       � ��$		 420��6.   �� 5<        �@ AlternatingRowBackground
	 �����6=   5=      5>      .+�5?   
   �� -   5        $��Right=�5?      .��5@      �� $��32��6   $��/Art/close.png��6    5A      � 5C   
   
 � ColumnDefinitions
 5D      �� 5E      $��* �6#   �� 5F      $
��auto �6#   �� 5G      $��* �6#   .+�5I      �� �� 5J      $��2��6   �� 5K      $��1��6   	 �� X2$ 90��6#   �� 5L   
   $ 
Horizontal=�6   $	��Top=�6.   $��1��6F   .+�5N      �� $��/Art/dragon.png��6   $	��100��6-   $��Center=�69   �� 5O      $��Checkout��6   $��40(�6(   $��Center=�66   $��Heavy"�6Q   $��Segoe UI Semibold)�6d   5P      5Q      �� 5S   
   $ 
Horizontal=�6   $��Center=�6.   $��0 10 0 0q�6K   .+�5T      �� $��/Art/paymethod.png��6   #��[  60   $��4q�6Y   �� 5U      -   5         �� Text$ Payment Method��5W      ��5U      �� *
PayMethods+ �� SelectedItem 5V      �� *SelectedPayMethod+ �� Mode$ OneWayToSource=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedIndex 5X      �� *PayMethSelIndex+5Y      �� 5Z   
   $ 
Horizontal=�6   $��Center=�6.   $��1��6K   .+�5[      �� $��/Art/deliveryTruck.png��6   #��[  64   $��4q�6]   �� 5\      -   5        $ Shipping Method��5^      ��5\      �� *ShipMethods+ 5]      �� *SelectedShipMethod+$ OneWayToSource=�$ PropertyChanged=� 5_      �� *ShipMethSelIndex+5`      �� 5a   
   $	 480��6   $��0 10 0 0q�6   �� 5b   
   $��Order Summary��6   $��Center=�6(   $��0q�6E   $��30(�6Q   $��Consolas)�6_     5c   
   ��6   �� *
CartList+ 5d        TextCellStyle��%)  &	       �� 5e      " ��6*   .�5f      ��  � ��$	 Center=�68   �� 5g       � ��$	 Center=�66   �� 5h       � ��$	 20(�6-   �� 5i       �@��Padding$	 2q�6,   �� 5j       � ��$
	 Wrap=�61   �� 5k       �@��TextAlignment$	 Center=�62   5l      5m         Columns 5n      0  *System.Windows.Controls.DataGridTextColumn  5o      1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6>   ,  &System.Windows.Controls.DataGridColumn   Width$ * 6l      Binding 6%   �� *Name+6l     5p      # [ 6B    6%   �� *
Quantity+6B     5q      # [ 6O    6%   �� *Price+ �� StringFormat$ c��6O   5s      �� 5t   
   .��5u      �� $ 
Horizontal=�6   $��0q�62   .+�5v      �� $��/Art/check.png��6   $��64��60   $��0q�6<   �� 5w      $��Finalize Order��6   $��Center=�61   $��28(�6L   $
��Bold"�6Z   5x      .  (System.Windows.Interactivity.Interaction   	Behaviors 5y      +	   %DragonDrop.WPF.EventToCommandBehavior	  5z       	  Event$ Click��6O    	  PassArguments .  	  Command 6-   �� *SubmitCommand+6]   5|      5}      5~      