   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   /clr-namespace:DragonDrop.WPFDragonDrop.WPF  5   	   e7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5	   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5
   	   A    ;DragonDrop.WPF.StoreViews.Tabs.SubViews.TakeInventoryWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5	   	   5my&clr-namespace:System;assembly=mscorlib     5
   	     � Title$  TakeInventoryWindow��5   	   $	��954��6%   $	��730��65    � WindowStartupLocation$ CenterScreen=�6A   $��0q�5   	   $��0q�6    � SizeToContent$ Height=�6     � 
ResizeMode$ NoResize=�67    � 	Resources 5       �� 
TargetType �� Property �� Value&   System.Windows.Controls.DataGridE   ?DragonDrop.WPF.StoreViews.Tabs.Converters.RawCodeToUpcConverter   upcFormatterM   GDragonDrop.WPF.StoreViews.Tabs.Converters.CodeOrProdToFontSizeConverter  CodeToFontSzI   CDragonDrop.WPF.StoreViews.Tabs.Converters.BoolToVisibilityConverter  BoolToVisConvJ   DDragonDrop.WPF.StoreViews.Tabs.Converters.InverseVisibilityConverter  
InvVisConv  HeaderImgSize=  7System.Windows.Controls.Primitives.DataGridColumnHeader  TextCellStyle  
baseTxtBox  CheckBtnStyle  CheckTxtBlkStyle 	 QtyBoxStyle �� BasedOn��%  '��       '  G     &	  �    &	 �    &	 �    &	 �    &	 �    '  �    &	 h    &	 �    '�� �    '�� �    &	 n    &	 O    &		 �	    7  �� 5   
   " ��6   .�5      ��  � ��$ 0q�6'   5      �� 5   
   "  6   .�5      ��  � ��$	 380��6*   �� 5       � ��$	 700��6&   �� 5       � ��$ 0q�6'   �� 5       � ��$ 0q�6(   �� 5       � �� 6+   Z� �� 5       �@ GridLinesVisibility$ Vertical=�64   �� 5       �@ CanUserAddRows .  6/   �� 5       �@ CanUserDeleteRows .  62   �� 5       �@ AutoGenerateColumns .  64   �� 5       � ��$ Center=�6;   �� 5       � ��$ Stretch=�64   �� 5        �@ AlternatingRowBackground
 �����69   �� 5       �@ SelectionMode$ Single=�6.   �� 5        �@ CanUserReorderColumns .  66   �� 5!       �@ CanUserResizeColumns .  65   �� 5"       �@ CanUserResizeRows .  62   �� 5#       �@ 
IsReadOnly . 6+   5$        5%   
   6%     5&   
   6-     5'   
   6)     5(   
   6*   X�5)   
   30Q�6.   �� 5*   
   "  6   .�5+      ��  � ��$ Center=�6;   �� 5,       � ��$ Center=�69   5-      �� 5.   
   " ��6&   .�5/      ��  � ��$ Center=�64   �� 50       � ��$ Center=�62   �� 51       � ��$ 18(�6)   �� 52       �@��Padding$	 1 0q�6(   �� 53       � ��$
 Wrap=�6-   �� 54       �@��TextAlignment$ Center=�6.   55      �� 56   
   " ��6#   .�57      ��  � ��$ 24(�6)   �� 58       �@��TextWrapping$
 Wrap=�6-   �� 59       �@��TextAlignment$ Center=�6.   �� 5:       � ��$ Center=�62   �� 5;       � ��$ Consolas)�6+   5<      �� 5=   
   " ��6   .�5>      ��  � ��$ Center=�6;   5?      �� 5@   
   " ��6   .�5A      ��  � ��$ Center=�64   �� 5B       � ��$
 Bold"�6+   �� 5C       � ��$ 24(�6)   5D      �� 5E   
   " ��6&   .�5F      ��  � ��$ Center=�6;   �� 5G       � ��$	 720��6&   �� 5H       � ��$ Center=�64   �� 5I       � ��$ 0q�6'   �� 5J       � ��$	 0 4q�6(   5K      �� 5L   
   " ��6)   .�5M      ��  � ��$ Stretch=�64   �� 5N       � ��$ Center=�62   �� 5O       � ��$	 500��6&   �� 5P       � ��$ 80��6*   �� 5Q       � ��$
 Wrap=�6-   �� 5R       �@��TextAlignment$ Center=�6.   �� 5S       � ��$ 20(�6)   �� 5T       � ��$
 Left=�6/   5U      �� 5V   
   " ��6$   8   .�5W      ��  � ��$	 4 8q�6(   �� 5X       � ��$ 80��6&   �� 5Y       � ��$ 32(�6)   �� 5Z       �@�Tag$ SelectableTxtBox��6$   5[      5\      .��5]      �� $	��720��6   $	��900��6   $��0q�6-   .+�5^   
   � $��60��6   
���OO/�.+�5_      �� 	 �� Orientation$	 
Horizontal=�6   $��Center=�62    5`      ��%S  '��       '�� �     '�� J    �� 5a      " ��6   .�5b      ��  � ��
 �����67   �� 5c       � ��$ Center=�6>   �� 5d       � ��$
 Bold"�67   �� 5e       � ��$ 24(�65   5f      �� 5g      " ��6   .�5h      ��  � ��
 ��=H�67   �� 5i       � ��
 �����67   �� 5j       � ��$ 34(�65   �� 5k       � ��$ Consolas)�67   �� 5l       � ��$	 140��62   �� 5m       � ��$ Center=�6>   �� 5n       �@��TextAlignment$ Center=�6:   �� 5o       � ��$ 10 0 0 0q�63   �� 5p       � ��$ 2q�6<   5q      �� 5r      " ��6   .�5s      ��  � �� 67   Z� �� 5t       � ��$ Center=�6>   �� 5u       � ��
 �����68   �� 5v       � ��$ 2q�6<   �� 5w       �@�ToolTip$ Set Port��64   5x      5y      .+�5z      �� $��Listening on Port:��6   �� 5{      -   5        ��5{      �� *Port+5|   8   �� 5}      .��5~      �� $��/Art/whiteSwap.png��6   $��44��68   .  (System.Windows.Interactivity.Interaction
   	Behaviors
 5      +   %DragonDrop.WPF.EventToCommandBehavior  5�         Event$ Click��5�   5      PassArguments .     Command 5�   5   �� *SetPortCommand+5�   C   5�      5�      5�      �� 5�   
   -   5         ��Name$ checkBtn��5�      #��[ 6B   {�6#   �� *
CanCheck+.��5�      ]� .+�5�      � $
��Left=�6   $��0 0 0 4q�6-   .+�5�      �� -   5         ��Name$ 	accArrow1��5�   <   $��/Art/arrow.png��6   $��32��60   y�5�      �� *
AcceptMode+ �� 	Converter# [ �� 5�      $��/Art/close.png��6   $��36��60   y�6<   ��  �� ElementName$ 	accArrow1�� �� Path$ 
Visibility�# [ 5�      � 5�      $��Right=�6   $��4 0 0 0q�6.   .+�5�      �� $��/Art/arrow.png��6   $��32��64   y�6A   �� $ 	accArrow1��$ 
Visibility��� 5�      $��/Art/close.png��6   $��36��64   y�6@   �� $ 	accArrow1��$ 
Visibility�# [ 5�      �� 5�      -   5         ��Name$ checkBtnTxtBlk��5�      #��[ 5�      ��5�   4   �� *	BtnText+5�      �� 5�      $��X��6   $��Center=�6$   ��5�      �� $ Content� �� RelativeSource �� *Self+# [ �� 5�      -   5        #��[	 5�      ��5�      �� *
CheckQty+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��5�   U   
 5�        5�      $ TextChanged��5�   5    .   5�   5   �� *QtyChangedCommand+5�   I   .��5�   /   5�      5�      
 5�        5�      $ Click��6N    .   6-   �� *CheckCommand+6\   5�        5�   
   -   5          Name$ 
incomingDG��5�      ��5�      �� *IncomingList+y�62   �� *
IncGridVis+   Columns 5�      0	  *System.Windows.Controls.DataGridTextColumn	  5�      1
  +System.Windows.Controls.DataGridBoundColumn 
  ElementStyle# [ 6>   ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 490 6l    
  Binding 6%   �� *Name+6l   	  5�      # [ 5�   %   $	 100 5�   %    	  
FontWeight$
 Bold"�61    5�   %   �� *	BarCode+# [  5�   1   	  5�      # [ 6B   $ 80 5�   %    5�   %   �� *
Quantity+5�   %   5�      � 5�   
   $��64��6   $��0q�6   
���OO/� 5�      ��%u   '��       �� 5�      " ��6   .�5�      ��  � �� 63   Z� �� 5�       � ��$ 2q�68   5�      5�       � ColumnDefinitions 5�      �� 5�      $��* �6#   �� 5�      $	��500 �6#   .+�5�      �� -   5        .��5�      �� $	 
Horizontal=�6   .+�5�      �� $��/Art/whiteCancelFull.png��6   $��48��6>   �� 5�      $��Center=�6   $��Consolas)�67   $��28(�6M   
�������.��6n   Cancel6v   5�      5�      �� 5�      $��1��6   {�6%   �� *	CanSubmit+.��5�      �� $	 
Horizontal=�6   .+�5�      �� $��/Art/whiteCheckFull.png��6   $��48��6=   �� 5�      $��Submit��6   $��Center=�6-   $��Consolas)�6H   
�������$��48(�6q   $��SemiBold"�6   5�      
 5�        5�      $ Click��5�   1    .   5�   1   �� *SubmitCommand+5�   ?   5�      5�      5�      5�      