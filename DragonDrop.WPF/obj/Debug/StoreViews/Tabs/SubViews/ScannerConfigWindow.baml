   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      I EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DragonDrop.WPFDragonDrop.WPF 5   	   A   ;DragonDrop.WPF.StoreViews.Tabs.SubViews.ScannerConfigWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      ,behclr-namespace:DragonDrop.WPF     5   	     � Title$  ScannerConfigWindow��5	   	    � SizeToContent$ Height=�6%   $	��320��6<    � WindowStartupLocation$ CenterScreen=�6H    � 
ResizeMode$ NoResize=�6m    � 	Resources 5
       �� 
TargetType   portLblStyle �� Property �� Value  receivedLblStyle��%|  &	        '�� �     '�� &    '�� Z    &	 ?    �� 5   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � ��$ Consolas)�6+   �� 5       � ��$ 28(�6)   5      �� 5   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � ��$ Center=�62   �� 5       � ��
 �����6+   �� 5       � ��$ 28(�6)   �� 5       � ��$ Consolas)�6+   �� 5       � ��$	 6 0q�6(   �� 5       � ��$	 110��6&   �� 5       � ��$ Center=�6;   �� 5       �@�Tag$ SelectableTxtBox��6$   5      �� 5   
   " ��6   .�5      ��  � ��
 �OO/�6+   �� 5       � ��
 �����6+   �� 5       � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5        � ��$ 24(�6)   �� 5!       � ��$
 20 0q�6(   �� 5"       � ��$ 14 0 0 0q�6'   5#      �� 5$   
   " ��6   .�5%      �� 
 �@��X1$ 0��6#   �� 5&      
 �@��X1$	 400��6#   �� 5'       � ��
 �����6'   �� 5(       � ��$ 8��60   �� 5)       � ��$ 0 10 0 0q�6'   5*      �� 5+   
   " ��6)   .�5,      ��  � ��$ Center=�62   �� 5-       � ��
 �����6+   �� 5.       � ��$	 160��6&   �� 5/       � ��$ Center=�6;   �� 50       � ��
 �OO/�6+   �� 51       � ��
 �����6+   52      53      .��54      ��  �� Orientation$ Vertical=�6   
����=H�.+�55   
   �� $ 
Horizontal=�6   $	��Top=�6.   .+�56      �� $��Port: ��6   #��[  6%   �� 57      -   5        	 ��Name$	 entryBox��57      ��59      �� *Port+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5:      $	��Set��6   .   (System.Windows.Interactivity.Interaction   	Behaviors 5;      +  %DragonDrop.WPF.EventToCommandBehavior  5<         Event$ Click��6P      PassArguments .     Command 61   �� *
SetCommand+6^   5>      5?      �� 5@      � 5A   
   
���#�k�.+�5C   
   �� $ 
Horizontal=�6   $��Center=�6.   .+�5D      �� $��Receiving: ��6   $��16(�6*   
�������68   �� 5E      #��[ 6/   ��6   �� *Status+6/   5F      5G      5H      5I      