   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   I EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DragonDrop.WPFDragonDrop.WPF 5   	   e7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters 5   	   ?$clr-namespace:DragonDrop.WPF.HelpersDragonDrop.WPF.Helpers 5	   	   a5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews 5
   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   @   :DragonDrop.WPF.StoreViews.Tabs.SubViews.ProcessOrderWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	   4hel$clr-namespace:DragonDrop.WPF.Helpers     5	   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5
   	   5my&clr-namespace:System;assembly=mscorlib     5   	     � Title$  Process Order��5   	   $	��954��6   $	��730��6/    � WindowStartupLocation$ CenterScreen=�6;   $��0q�5   	   $��0q�6    � SizeToContent$ Height=�6     � 
ResizeMode$ NoResize=�67    � 	Resources 5       �� 
TargetType �� Property �� Value&   System.Windows.Controls.DataGridE  ?DragonDrop.WPF.StoreViews.Tabs.Converters.RawCodeToUpcConverter   upcFormatter  HeaderImgSize=  7System.Windows.Controls.Primitives.DataGridColumnHeader  TextCellStyle  
baseTxtBox  CheckBtnStyle  CheckTxtBlkStyle  QtyBoxStyle �� BasedOn��%�
  '��       '  G     &	  �    &	 �    '  �    &	 /    &	 P    '�� S    '�� �    &	 5    &	     &	 z	    7  �� 5   
   " ��6   .�5      ��  � ��$ 0q�6'   5      �� 5   
   "  6   .�5      ��  � ��$	 380��6*   �� 5       � ��$	 700��6&   �� 5       � ��$ 0q�6'   �� 5       � ��$ 0q�6(   �� 5       � �� 6+   Z� �� 5       �@ GridLinesVisibility$ Vertical=�64   �� 5       �@ CanUserAddRows .  6/   �� 5       �@ CanUserDeleteRows .  62   �� 5       �@ AutoGenerateColumns .  64   �� 5       � ��$ Center=�6;   �� 5       � ��$ Stretch=�64   �� 5        �@ AlternatingRowBackground
 �����69   �� 5        �@ SelectionMode$ Single=�6.   �� 5!       �@ CanUserReorderColumns .  66   �� 5"       �@ CanUserResizeColumns .  65   �� 5#       �@ CanUserResizeRows .  62   �� 5$       �@ 
IsReadOnly . 6+   5%        5&   
   6%   X�5'   
   30Q�6.   �� 5(   
   "  6   .�5)      ��  � ��$ Center=�6;   �� 5*       � ��$ Center=�69   5+      �� 5,   
   " ��6&   .�5-      ��  � ��$ Center=�64   �� 5.       � ��$ Center=�62   �� 5/       � ��$ 18(�6)   �� 50       �@��Padding$	 1 0q�6(   �� 51       � ��$
 Wrap=�6-   �� 52       �@��TextAlignment$ Center=�6.   53      �� 54   
   " ��6#   .�55      ��  � ��$ 24(�6)   �� 56       �@��TextWrapping$
 Wrap=�6-   �� 57       �@��TextAlignment$ Center=�6.   �� 58       � ��$ Center=�62   �� 59       � ��$ Consolas)�6+   5:      �� 5;   
   " ��6   .�5<      ��  � ��$ Center=�6;   5=      �� 5>   
   " ��6   .�5?      ��  � ��$ Center=�64   �� 5@       � ��$
 Bold"�6+   �� 5A       � ��$ 24(�6)   5B      �� 5C   
   " ��6&   .�5D      ��  � ��$ Center=�6;   �� 5E       � ��$	 720��6&   �� 5F       � ��$ Center=�64   �� 5G       � ��$ 0q�6'   �� 5H       � ��$	 0 4q�6(   5I      �� 5J   
   " ��6)   .�5K      ��  � ��$ Stretch=�64   �� 5L       � ��$ Center=�62   �� 5M       � ��$	 500��6&   �� 5N       � ��$ 80��6*   �� 5O       � ��$
 Wrap=�6-   �� 5P       �@��TextAlignment$ Center=�6.   �� 5Q       � ��$ 20(�6)   �� 5R       � ��$
 Left=�6/   5S      �� 5T   
   " ��6$   8   .�5U      ��  � ��$	 4 8q�6(   �� 5V       � ��$ 80��6&   �� 5W       � ��$ 32(�6)   �� 5X       �@�Tag$ SelectableTxtBox��6$   5Y      5Z      .��5[      �� $	��720��6   $	��900��6   $��0q�6-   .+�5\   
   � $��60��6   
���OO/�.+�5]   
   �� 	 �� Orientation$	 
Horizontal=�6   $��Center=�6.    5^      ��%S  '��       '�� �     '�� J    �� 5_      " ��6   .�5`      ��  � ��
 �����67   �� 5a       � ��$ Center=�6>   �� 5b       � ��$
 Bold"�67   �� 5c       � ��$ 24(�65   5d      �� 5e      " ��6   .�5f      ��  � ��
 ��=H�67   �� 5g       � ��
 �����67   �� 5h       � ��$ 34(�65   �� 5i       � ��$ Consolas)�67   �� 5j       � ��$	 140��62   �� 5k       � ��$ Center=�6>   �� 5l       �@��TextAlignment$ Center=�6:   �� 5m       � ��$ 10 0 0 0q�63   �� 5n       � ��$ 2q�6<   5o      �� 5p      " ��6   .�5q      ��  � �� 67   Z� �� 5r       � ��$ Center=�6>   �� 5s       � ��
 �����68   �� 5t       � ��$ 2q�6<   �� 5u       �@�ToolTip$ Set Port��64   5v      5w      .+�5x      �� $��Listening on Port:��6   �� 5y      -   5        ��5y      �� *Port+5z   8   �� 5{      .��5|      �� $��/Art/whiteSwap.png��6   $��44��68   .   (System.Windows.Interactivity.Interaction
   	Behaviors
 5}      +  %DragonDrop.WPF.EventToCommandBehavior  5~         Event$ Click��5   5      PassArguments .     Command 5~   5   �� *SetPortCommand+5   C   5�      5�      5�        5�   
   -   5          Name$ unpDG��5�      ��5�      �� *UnprocessedList+y�65   �� *UnProcGridVis+   Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6>   ,  &System.Windows.Controls.DataGridColumn   Width5	  /System.Windows.Controls.DataGridLengthConverter$	 490	 6l      Binding 6%   �� *Name+6l     5�      # [ 5�   %   $	 100	 5�   %      
FontWeight$
 Bold"�61    5�   %   �� *	BarCode+ �� 	Converter# [  5�   1     5�      # [ 6B   $ 80	 5�   %    5�   %   �� *
Quantity+5�   %   5�      �� 5�   
   -   5         ��Name$ checkBtn��5�      #��[ 6B   {�6#   �� *
CanCheck+.��5�      ]� .+�5�      �� $��/Art/arrow.png��6   $��32��60   $
��Left=�6<   $��0 0 0 4q�6R   �� 5�      $��/Art/arrow.png��6   $��32��60   $��Right=�6<   $��4 0 0 0q�6S   �� 5�      -   5         ��Name$ checkBtnTxtBlk��5�      #��[ 5�      ��5�   4   �� *CurScannedTitle+5�      �� 5�      $��X��6   $��Center=�6$   �� 5�      -   5        #��[ 5�      ��5�      �� *
CheckQty+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��5�   U   
 5�        5�      $ TextChanged��5�   5    .   5�   5   �� *QtyChangedCommand+5�   I   .��5�   /   5�      5�      
 5�        5�      $ Click��6N    .   6-   �� *CheckCommand+6\   5�        5�   
   -   5        $ procDG��5�      ��5�      �� *ProcessedList+y�63   �� *ProcGridVis+ 5�   
     5�      # [ 6>   $	 490	 6l    6%   �� *Name+6l     5�      # [ 5�   %   $	 100	 5�   %   $
 Bold"�61    5�   %   �� *	BarCode+# [  5�   1     5�      # [ 6B   $ 80	 5�   %    5�   %   �� *
Quantity+5�   %   5�      � 5�   
   $��64��6   $��0q�6   
���OO/� 5�      ��%u   '��       �� 5�      " ��6   .�5�      ��  � �� 63   Z� �� 5�       � ��$ 2q�68   5�      5�       � ColumnDefinitions 5�      �� 5�      $��* �6#   �� 5�      $	��500 �6#   .+�5�   
   �� -   5        .��5�      �� $	 
Horizontal=�6   .+�5�      �� $��/Art/whiteCancelFull.png��6   $��48��66   �� 5�      $��Center=�6   $��Consolas)�6/   $��28(�6E   
�������.��6f   Cancel6n   5�      5�      �� 5�      $��1��6   {�6%   �� *	CanSubmit+.��5�      �� $	 
Horizontal=�6   .+�5�      �� $��/Art/whiteCheckFull.png��6   $��48��6=   �� 5�      $��Submit��6   $��Center=�6-   $��Consolas)�6H   
�������$��48(�6q   $��SemiBold"�6   5�      
 5�        5�      $ Click��5�   1    .   5�   1   �� *SubmitCommand+5�   ?   5�      5�      5�      5�      