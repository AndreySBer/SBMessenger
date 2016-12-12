// NativeLinker.cpp : Defines the exported functions for the DLL application.
//

//#include "stdafx.h"

#include "messenger\callbacks.h"
#include "messenger\messenger.h"
#include "messenger\observers.h"
#include "messenger\settings.h"
#include "messenger\types.h"
#include "NativeLinker.h"

using namespace messenger;

typedef unsigned char * BYTES;

typedef void(*LoginCallback)(messenger::operation_result::Type);
class CallbackProxy : public messenger::ILoginCallback
{
public:
	LoginCallback loginCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result) override {
		if (loginCallback != NULL) loginCallback(result);
		loginCallback = NULL;
	}
};

typedef void(*StatusChanged)(char* MessageId, message_status::Type);
//todo add time into method
typedef void(*MessageReceived)(char* UserId,
	char* MessageId,
	std::time_t time,
	/*MessageContent*/
	message_content_type::Type type,
	bool encrypted,
	unsigned char* Data, int len);

unsigned char * vecToArray(std::vector<unsigned char> vec);

class MessagesObserver :public messenger::IMessagesObserver {
public:
	StatusChanged statusChanged = NULL;
	MessageReceived messageReceived = NULL;
private:
	void OnMessageStatusChanged(const MessageId& msgId, message_status::Type status)override {
		if (statusChanged != NULL) {
			statusChanged((char*)(msgId.c_str()), status);
		}
		statusChanged = NULL;
	}
	void OnMessageReceived(const UserId& senderId, const Message& msg)override {
		if (messageReceived != NULL) {
			messageReceived((char*)(senderId.c_str()),
				(char*)(msg.identifier.c_str()),
				msg.time,
				msg.content.type,
				msg.content.encrypted,
				vecToArray(msg.content.data),
				msg.content.data.size());
		}
		messageReceived = NULL;
	}
};
struct UserFull
{
	char* userID;
	encryption_algorithm::Type encryptionAlgo;
	unsigned char* SecPublicKey;
	int KeyLength;
};
UserFull* usersArr;
int usersCount;
typedef void(*UsersResultCallback)(char** users, int length);
class CallbackRequestUser : public messenger::IRequestUsersCallback
{
public:
	UsersResultCallback callback = NULL;
private:
	void OnOperationResult(operation_result::Type result, const UserList& users) override {
		if (usersArr) {
			delete[] usersArr;
		}
		usersCount = users.size();
		usersArr = new UserFull[users.size()];
		char** res = new char*[users.size()];
		for (int i = 0; i < users.size(); i++) {
			res[i] = (char*)(users[i].identifier).c_str();
			usersArr[i].userID = (char*)(users[i].identifier).c_str();
			usersArr[i].encryptionAlgo = users[i].securityPolicy.encryptionAlgo;
			usersArr[i].SecPublicKey = vecToArray(users[i].securityPolicy.encryptionPubKey);
			usersArr[i].KeyLength = users[i].securityPolicy.encryptionPubKey.size();
		}
		if (callback != NULL) callback(res, users.size());
		callback = NULL;
	}
};


static std::shared_ptr<messenger::IMessenger> g_messenger;
static CallbackProxy g_callbackProxy;
static MessagesObserver g_messagesObserver;
static CallbackRequestUser g_callbackReqUser;


std::vector<unsigned char> arrToVector(unsigned char* arr, int size) {
	std::vector<unsigned char> res = std::vector<unsigned char>(size);
	for (int i = 0; i < size; i++) {
		res[i] = arr[i];
	}
	return res;
}

unsigned char* vecToArray(std::vector<unsigned char> vec) {
	unsigned char* res = new unsigned char[vec.size()];
	for (int i = 0; i < vec.size(); i++) {
		res[i] = vec[i];
	}
	return res;
}

extern "C" {
	void __declspec(dllexport) Init(char* url, unsigned short port) {
		messenger::MessengerSettings settings;
		settings.serverPort = port;
		settings.serverUrl = url;
		g_messenger = messenger::GetMessengerInstance(settings);
	}

	void __declspec(dllexport) Disconnect() {
		g_messenger->UnregisterObserver(&g_messagesObserver);
		g_messenger->Disconnect();
		g_messenger->~IMessenger();
	}

	void __declspec(dllexport) Login(char * login, char * password, LoginCallback loginCallback) {
		std::string loginStr(login);
		std::string passwordStr(password);
		g_callbackProxy.loginCallback = loginCallback;
		g_messenger->Login(loginStr, passwordStr, messenger::SecurityPolicy(), &g_callbackProxy);
	}

	void __declspec(dllexport) RequestActiveUsers(UsersResultCallback reqUserCallback) {
		g_callbackReqUser.callback = reqUserCallback;
		g_messenger->RequestActiveUsers(&g_callbackReqUser);
	}


	void _declspec(dllexport) SendMessage(char* recepientId, char* msg, int msg_len) {
		std::vector<unsigned char>data(msg, msg + msg_len);
		Data datad = (Data)data;
		MessageContent msgData = MessageContent();
		msgData.type = message_content_type::Text;
		msgData.data = datad;
		g_messenger->SendMessage(recepientId, msgData);
	}

	void _declspec(dllexport) SendComplexMessage(char* recepientId,
		/*MessageContent*/
		message_content_type::Type type,
		bool encrypted,
		unsigned char* msg,
		int msg_len) {
		MessageContent msgC;
		msgC.type = type;
		msgC.encrypted = encrypted;
		msgC.data = arrToVector(msg, msg_len);
		g_messenger->SendMessage(recepientId, msgC);
	}

	void _declspec(dllexport) SendMessageSeen(char* userId, char* msgId) {
		g_messenger->SendMessageSeen(userId, msgId);
	}

	void __declspec(dllexport) RegisterObserver(StatusChanged statusChanged, MessageReceived messageReceived) {
		g_messagesObserver.statusChanged = statusChanged;
		g_messagesObserver.messageReceived = messageReceived;
		g_messenger->RegisterObserver(&g_messagesObserver);
	}

	encryption_algorithm::Type __declspec(dllexport) GetUserEncryption(char* userId) {
		for (int i = 0; i < usersCount; i++) {
			if (usersArr[i].userID = userId) {
				return usersArr[i].encryptionAlgo;
			}
		}
		return encryption_algorithm::Type::None;
	}
	

	BYTES __declspec(dllexport) GetPublicKey(char * userId) {
		for (int i = 0; i < usersCount; i++) {
			if (usersArr[i].userID = userId) {
				return usersArr[i].SecPublicKey;
			}
		}
		return 0;
	}
}
