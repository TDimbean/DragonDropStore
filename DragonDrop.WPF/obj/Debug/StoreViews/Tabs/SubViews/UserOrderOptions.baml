   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   a5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   >    8DragonDrop.WPF.StoreViews.Tabs.SubViews.UserOrderOptions   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � Title$  Order��5	   	   $	��480��6    � SizeToContent$ Height=�6#    � WindowStartupLocation$ CenterScreen=�6:    � 
ResizeMode$ NoResize=�6_    � 	Resources 5
         HoverlessBtnStyle �� 
TargetType �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value �� 
TargetName  	InfoStyle  InfoBorderBrush  
ArrowStyle �� ScaleX �� ScaleY&   System.Windows.Controls.DataGrid  ReportBtnStyle  InfoGridStyleJ   DDragonDrop.WPF.StoreViews.Tabs.Converters.IdToPaymentMethodConverter  IdToPayMethConvK   EDragonDrop.WPF.StoreViews.Tabs.Converters.IdToShippingMethodConverter  IdToShipMethConvH   BDragonDrop.WPF.StoreViews.Tabs.Converters.IdToOrderStatusConverter  IdToStatMethConv 	 
DashedLine �� StrokeDashArray	 �� X1	 �� Y1	 �� X2	 �� Y2��%T
  &	        &	 �    &	 o    '��     &	 {    '  �    &	 �    &	 �    &	 *	    &	 =	    &	 P	    &		 c	    �� 5   
   " ��6*   .�5      ��  �@�OverridesDefaultStyle . 66   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��border��6!   ��6/   Z� ._�5      �� $��Center=�6/   $��Center=�6L   5      	 5      P� 5      
 �@J�IsMouseOver . . �5   "   �� $ border��6)    6T   Z�  � ��5      5      5      5      �� 5   
   " ��6"   .�5      ��  � ��$ Center=�64   �� 5       � ��$ 18(�6)   �� 5       � ��
 ����6+   �� 5        � ��$ 4q�6'   �� 5!       � ��$
 Wrap=�6-   5"      �� 5#   
   " ��6(   .�5$      ��  � ��$ 2q�60   �� 5%       � ��$ Center=�64   �� 5&       �@��CornerRadius$ 12��6-   5'      �� 5(   
   " ��6   .�5)      ��  � �� 6,   Z� �� 5*       � �� 6+   Z� 5+      �� 5,   
   " ��6#   .�5-      ��  � ��$* $pack://application:,,,/Art/arrow.png��6'   �� 5.       � ��$ 26��6'   �� 5/       �@J�RenderTransformOrigin$ .5 .5&�66   �� 50       � z� 51      �� 52      $ 1Q�6%   $ -1Q�60   54      55      �� 56   
   "  6   .�57      ��  �@ GridLinesVisibility$ Vertical=�64   �� 58       �@ AutoGenerateColumns .  64   �� 59       � ��$ Center=�64   �� 5:       � ��$ Center=�6;   �� 5;       �@ SelectionMode$ Single=�6.   �� 5<       �@ CanUserAddRows .  6/   �� 5=       �@ CanUserDeleteRows .  62   �� 5>       �@ CanUserReorderColumns .  66   �� 5?       �@ CanUserResizeColumns .  65   �� 5@       �@ CanUserResizeRows .  62   �� 5A       �@ 
IsReadOnly . 6+   �� 5B       � ��$	 400��6*   �� 5C        �@ AlternatingRowBackground
 � ���69   �� 5D       �@ HeadersVisibility$
 None=�62   5E      �� 5F   
   " ��6'   .�5G      ��  � ��$ Consolas)�6+   �� 5H       � ��$ 30(�6)   �� 5I       � ��$	 0 6q�6'   �� 5J       � �� 6+   Z� �� 5K       � �� 6,   Z� �� 5L       � ��$ Center=�64   5M      �� 5N   
   " �6&   .�5O      ��  � ��$ 1��6)   �� 5P       � ��$ Center=�64   �� 5Q       � ��$ 0 0 0 20q�6'   5R        5S   
   6*     5T   
   6+     5U   
   6(   �� 5W   
   $ 2R�6"   $ 0��66   $ 0��6=   $	 480��6D   $ 0��6M   
���   �$��4��6c   
���   �6w   5X      .��5Z      �� .+�5[   
   ]� .+�5\      �� $.��(pack://application:,,,/Art/orderitem.png��6   $��90��6F   $
��Left=�6R   � 5]       5^      ��%�   '��       �� 5_      " ��6   .�5`      ��  � ��$ Center=�6E   �� 5a       � ��$ Center=�6>   �� 5b       � ��$ 20(�65   5c      5d       � ColumnDefinitions 5e      �� 5f      $��* �6'   �� 5g      $
��auto �6'    � RowDefinitions 5i      �� 5j      $��* �6$   �� 5k      $��* �6$   .+�5m      ��  �� Orientation$ 
Horizontal=�6   .+�5n      �� $��
Placed On:��6   $��Center=�61   �� 5o      ��6   �� *	OrderDate+ �� StringFormat$ d��5p      �� 5q      $ 
Horizontal=�6   $��1��67   .+�5r      �� $��Shipped On:��6   $��Center=�62   �� 5s      ��6   �� *ShippingDate+5t      �� 5u      $ 
Horizontal=�6   $��1��62    � ToolTip$ Payment Method��6B   .+�5v      �� $.��(pack://application:,,,/Art/payMethod.png��6   $��30��6N   �� 5w      ��6   �� *PaymentMethodId+ �� 	Converter# [ 5x      �� 5y      $ 
Horizontal=�6   $��1��62   $��1��6B   $ Delivery Method��6O   .+�5z      �� $2��,pack://application:,,,/Art/deliveryTruck.png��6   $��30��6R   �� 5{      ��6   �� *ShippingMethodId+# [ 5|      5}      5~      �� 5   
   $��Center=�5�      $ Status��6-   $��Center=�5�      $��20(�6+   $��Consolas)�69   ��5      �� *OrderStatusId+# [ 5�   9   �� 5�   
   -   5        $ Hide Products��5�      #��[  6*   $��0 0 0 4q�5�   (   .��5�      �� -   5         ��Name$ 
ProdsArrow��5�      #��[ 6(   5�        5�   
   -   5        $��custDG��5�   j   $	��468��6x   ��6   �� *ProductsList+ �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=� 5�       
 TextCellStyle��%�   &	
       �� 5�      " ��62   .�5�      ��  � ��$ Center=�6@   �� 5�       � ��$ Center=�6>   �� 5�       � ��$ 20(�65   �� 5�       � ��$
 Wrap=�69   �� 5�       �@��TextAlignment$ Center=�6:   5�      5�         Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��6%   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [
 5�   %       MaxWidth$	  300Q�5�   %   !   Binding! 5�   3   �� *Name+5�   %     5�      $	 Qty��6%   # [
 5�   %   "   MinWidth$" 50Q�5�   %   ! 5�   2   �� *
Quantity+5�   %     5�      $ Price��6%   # [
 5�   %   $" 84Q�5�   %   ! 5�   4   �� *Price+$ c��5�   %   4  .System.Windows.Controls.DataGridTemplateColumn  5�      #   Width5	  /System.Windows.Controls.DataGridLengthConverter$# 30	 6)   $   CellTemplate$ 5�      �� 5�      .R�5�      �� -   5        .��5�   "   �� $-��'pack://application:,,,/Art/infoFull.png��6(   $��30��5�   (   5�      5�      5�      5�      �� 5�   
   $ 2R�6   $ 0��6#   $ 0��6*   $	 480��61   $ 0��6:   
���   �$��4��6P   
���   �6d   �� 5�   
   -   5        $��Request Modifications��5�      #��[ 6G   � 5�      #��[ 6   .+�5�      �� #��[ 6   ��5�      L� 5�      % j� Viewport$% 0,0,8,8�6'   & j� ViewportUnits$& Absolute=�6:   ' j� TileMode$
' Tile=�6S   ( L� Drawing( 5�      I� 5�   "   .��5�   &   � ) � Brush
) ���_�* � Geometry* 5�   *   � 5�   .   .��5�   2   �� + �� Rect$+ 	0,0,50,50�6D   �� 5�   2   $+ 50,50,50,50�6D   5�   /   5   '   5  #   5     ._�5     ]� .+�5     �� $��4q�6    $)��#pack://application:,,,/Art/info.png��6+   $��30��6X   $	��Top=�6d   �� 5     $L��FNeed a last minute change? Let us know and we'll make sure you get it.��6$   , �� TextAlignment$, Center=�5     #��[ 63   5	     5
     5     �� 5  
   $ 2R�6   $ 0��6#   $ 0��6*   $	 480��61   $ 0��6:   
���   �$��4��6P   
���   �6d   �� 5  
   -   5        $��Report Issue��5     #��[ 6?   � 5     #��[ 6   $��0q�6:   .+�5     �� #��[ 6   ��5     L� 5     $% 0,0,8,8�6'   $& Absolute=�6:   $
' Tile=�6S   ( 5     I� 5  "   .��5  &   � 
) ���_�* 5  *   � 5  .   .��5  2   �� $+ 	0,0,50,50�6D   �� 5  2   $+ 50,50,50,50�6D   5  /   5  '   5  #   5     ._�5!     ]� .+�5"     �� $��4q�6    $)��#pack://application:,,,/Art/info.png��6+   $��30��6X   $	��Top=�6d   �� 5#     $����If you are for any reason unsatisfied with the package you received, just let us know and we'll cover a replacement and the shipping costs.��6$   $, Center=�5$     #��[ 63   5%     5&     5'     �� 5(  
   -   5        .��5)     �� $ 
Horizontal=�6   .+�5*     �� $*��$pack://application:,,,/Art/close.png��6   $��36��6F   �� 5+     $
��Done��6   $��Consolas)�6'   $��24(�6=   5,     5-     5.     5/     