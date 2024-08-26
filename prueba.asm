;Autor: Vega Angeles Christopher
;Fecha:21/08/2024 08:57:57 a. m.
include 'emu8086.inc'
org 100h
print "Proyecto 6"
printn " - ITQ"
printn ' ' 
call scan_num
MOV e , CX
MOV AX , 3
PUSH AX
MOV AX , 5
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
MOV AX , 8
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX , 10
PUSH AX
MOV AX , e
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX , 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
MOV pi , AX
INC pi
DEC e
MOV  AX, 4
ADD pi , AX
SUB pi , 5
SUB e , 3
MOV  AX, 10
MUL pi
MOV pi , AX
ADD e , 2
MOV  BX, 2
MOV  AX, 615
DIV BX
MOV pi , AX
MOV AX , 1
PUSH AX
MOV AX , 1
PUSH AX
MOV AX , 2
PUSH AX
MOV AX , 2
PUSH AX
printn "EntrÃ³ al IF"
MOV AX , 100
PUSH AX
POP AX
MOV a , AX
MOV AX , 200
PUSH AX
;Variables
;---------
e dw 0
pi dw 0
a dw 0
ret
define_scan_num
define_print_num_uns 
define_print_num
end
