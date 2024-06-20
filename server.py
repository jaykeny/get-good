import usb.core
import usb.util
import sys
import threading
import time
import socket

# Replace these with your actual device's vendor and product ID
VENDOR_ID = 0x1038
PRODUCT_ID = 0x2202

# Find the USB device
dev = usb.core.find(idVendor=VENDOR_ID, idProduct=PRODUCT_ID)

if dev is None:
    raise ValueError('Device not found')

# Set the active configuration
dev.set_configuration()

# Example: Assume the endpoint you need is the first endpoint of the fifth interface of the first configuration
interface_number = 5
endpoint_address = 0x86

# Find the specific interface by its number
interface = None
for cfg in dev:
    for intf in cfg:
        if intf.bInterfaceNumber == interface_number:
            interface = intf
            break
    if interface is not None:
        break

if interface is None:
    raise ValueError(f"Couldn't find interface {interface_number}")

# Find the specific endpoint on the interface
endpoint = usb.util.find_descriptor(
    interface,
    custom_match=lambda e: \
        usb.util.endpoint_direction(e.bEndpointAddress) == usb.util.ENDPOINT_IN and \
        e.bEndpointAddress == endpoint_address
)

if endpoint is None:
    raise ValueError(f"Couldn't find endpoint {endpoint_address} on interface {interface_number}")

# Event to signal thread termination
exit_event = threading.Event()

# Thread function for USB reading
def usb_reader():
    try:
        while not exit_event.is_set():
            try:
                data = dev.read(endpoint.bEndpointAddress, endpoint.wMaxPacketSize, timeout=0)  # Non-blocking read
                if data is not None:
                    # Extract voice and system levels from the received data
                    voice_level  = data[2]  
                    system_level = data[1]

                    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

                    # Connect to the C# server (adjust host and port accordingly)
                    server_address = ('localhost', 8089)
                    client_socket.connect(server_address)

                    # Data should only be sent if one of the levels is 100, because for some reason sometimes the headset sends an incorrect amount
                    if voice_level == 100 or system_level == 100:
                        # Send Data
                        data = f"{voice_level},{system_level}".encode()

                        # Send data
                        client_socket.sendall(data)

                        # Close the socket
                        client_socket.close()
                        
                        # Print the extracted levels
                        print(f'Voice Level: {voice_level}, System Level: {system_level}')
                    else:
                        print('Voice or System level not at 100, ignoring this from the headset.')
            except usb.core.USBError as e:
                if e.errno == 110:  # errno 110 is a timeout error
                    continue  # Non-blocking read, continue to check for KeyboardInterrupt
                elif e.errno == 19:  # errno 19 is no such device error
                    # print("USB device disconnected.")
                    break  # Exit the loop on device disconnection
                else:
                    print(f"USB error: {e}")
    except KeyboardInterrupt:
        print("Exiting...")
    except Exception as e:
        print(f"Unexpected error in USB thread: {e}")
    finally:
        usb.util.dispose_resources(dev)

# Start USB reading thread
usb_thread = threading.Thread(target=usb_reader)
usb_thread.start()

try:
    # Wait for KeyboardInterrupt
    while usb_thread.is_alive():
        usb_thread.join(timeout=0.1)  # Check thread status with timeout
except KeyboardInterrupt:
    print("Exiting...")
    # Set exit event to stop the USB thread
    exit_event.set()
    usb_thread.join()
except Exception as e:
    print(f"Unexpected error: {e}")
finally:
    # Ensure the device is properly released
    usb.util.dispose_resources(dev)

    # Wait for USB thread to complete if it's still running
    if usb_thread.is_alive():
        usb_thread.join()

    # Explicitly exit the program to prevent finalizing Python errors
    sys.exit(0)
