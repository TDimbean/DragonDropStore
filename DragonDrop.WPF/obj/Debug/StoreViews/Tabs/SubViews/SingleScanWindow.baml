   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   I EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DragonDrop.WPFDragonDrop.WPF 5   	   a5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews 5   	   >   8DragonDrop.WPF.StoreViews.Tabs.SubViews.SingleScanWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � Title$  SingleScanWindow��5
   	   $	��640��6"    � SizeToContent$ Height=�6.    � WindowStartupLocation$ CenterScreen=�6E    � 
ResizeMode$ NoResize=�6j   
���OO/� � 	Resources 5         PortBoxStyle �� 
TargetType �� Property �� Value  ListeningLblStyle  CodeLblStyle��%�  &	        &	 �    '�� �    &	 h    �� 5   
   " ��6%   .�5      ��  � ��
 �#�k�6+   �� 5       � ��
 �����6+   �� 5       �@��TextAlignment$ Center=�6.   �� 5       � ��$ Center=�62   �� 5       � ��$ Center=�69   �� 5       � ��$ Center=�6;   �� 5       � ��$ 38(�6)   �� 5       � ��$ Consolas)�6+   �� 5       � ��$
 10 0q�6(   �� 5       � ��$	 160��6&   �� 5       �@�Tag$ SelectableTxtBox��6$   5      �� 5   
   " ��6*   .�5      ��  � ��$ Consolas)�6+   �� 5       � ��$ 36(�6)   �� 5       � ��$ Center=�64   �� 5       � ��
 �����6+   5      �� 5    
   " ��6   .�5!      �� 
 �@��X1$ 0��6#   �� 5"      
 �@��X2$	 700��6#   �� 5#       � ��
 �����6'   �� 5$       � ��$ 4��60   5%      �� 5&   
   " ��6%   .�5'      ��  � ��$ Consolas)�6+   �� 5(       � ��$ 36(�6)   �� 5)       � ��$ Center=�64   �� 5*       � ��$ Center=�6;   �� 5+       � ��
 ��d�6+   �� 5,       � ��
 �����6+   �� 5-       � ��$	 320��6&   �� 5.       � ��$
 0 10q�6'   5/      50      .��51      �� .+�52   
   ��  �� Orientation$ 
Horizontal=�6   $��Center=�6.   $
��0 10q�6K   .+�53   
   �� $��Listening on Port: ��6   #��[ 6.   �� 54      -   5        #��[  54   ,   ��6   �� *Port+55   8   �� 56      	 � ToolTip$	 Change Port��6   
�������$��2q�6U   ��6+   Z� .   (System.Windows.Interactivity.Interaction
   	Behaviors
 57      +  %DragonDrop.WPF.EventToCommandBehavior  58         Event$ Click��6T      PassArguments .     Command 61   �� *SetPortCommand+6b   .��5:      �� $��/Art/whiteSwap.png��6   $��32��64   $	��6 0q�6@   5;      5<      �� 5=   
   �� 5>   
   -   5         ��Name$ codeBox��5>      #��[ 6:   ��6!   �� *Code+6:   � 5?   
   
���/kU� 5@      ��%�  '��       '�� H     '�� �     �� 5A      " ��6   .�5B      ��  � ��$ 32��6/   5C      �� 5D      " ��6   .�5E      ��  � ��$ Center=�6:   �� 5F       � ��$ 24(�61   �� 5G       � ��
 �����63   5H      �� 5I      " ��6   .�5J      ��  � �� 63   Z� �� 5K       � ��
 �����64   �� 5L       � ��$ 2q�68   5M      5N       � ColumnDefinitions 5O      �� 5P      $��* �6#   �� 5Q      $��* �6#   �� 5R      $��* �6#   .+�5T      �� -   5        .��5U      �� $ 
Horizontal=�6   .+�5V      �� $��/Art/whiteCancelFull.png��6   �� 5W      $��Cancel��6   5X      5Y      �� 5Z      $��1��6   $��2��6%   .��5[      �� $ 
Horizontal=�6   .+�5\      �� $��/Art/whiteCheckFull.png��6   �� 5]      $��Use This Code��6   5^      
 5_        5`      $ Click��5a   1    .   5`   1   �� *SubmitCommand+5a   ?   5c      5d      5e      5f      