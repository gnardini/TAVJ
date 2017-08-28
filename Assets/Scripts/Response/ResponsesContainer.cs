using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsesContainer : ServerResponse {
	
	private byte[] _data;

	public ResponsesContainer(byte[] data) {
		_data = data;
	}

	public static ResponsesContainer FromBytes(BitBuffer bitBuffer) {
		int size = bitBuffer.GetAvailableByteCount() - 1;
		byte[] bytes = new byte[size];
		for (int i = 0; i < bytes.Length; i++) {
			bytes[i] = bitBuffer.GetByte();
		}
		return new ResponsesContainer (bytes);
	}

	public byte[] GetData() {
		return _data;
	}

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
		bitBuffer.PutBytes(_data);
	}

	public override ResponseType GetResponseType() {
		return ResponseType.RESPONSES_CONTAINER;
	}

}
